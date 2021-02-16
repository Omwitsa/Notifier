using System;
using System.Collections.Generic;
using System.Linq;
using AbnNotifier.Data.Notifier;
using AbnNotifier.Data.Notifier.Models;
using AbnNotifier.Data.Unisol;
using AbnNotifier.Data.Unisol.Models;
using AbnNotifier.Transfer;
using AbnNotifier.Transfer.Unisol;
using Microsoft.EntityFrameworkCore;

namespace AbnNotifier.Services
{
    public class UnisolService
    {
        private readonly UnisolDbContext _unisolDbContext;
        private readonly AbnNotifierDbContext _notifierDbContext;
        public UnisolService(string conString, AbnNotifierDbContext notifierDbContext)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnisolDbContext>();
            optionsBuilder.UseSqlServer(conString);

            _unisolDbContext = new UnisolDbContext(optionsBuilder.Options);
            _notifierDbContext = notifierDbContext;
        }

        public UnisolResponse<UniEmployee> GetByEmpNo(string empNo)
        {
            var result = new UnisolResponse<UniEmployee> { Message = "Employee not found" };
            try
            {
                var employee = _unisolDbContext.HrpEmployees.FirstOrDefault(e => e.EmpNo.Equals(empNo));
                if (employee == null)
                    return result;
                result.Success = true;
                result.Message = "Employee found";
                result.Data = new UniEmployee(employee);
                return result;
            }
            catch (Exception e)
            {
                result.Message = $"Failed to get employee {empNo}";
                result.ErrorMessage = e.Message;
                return result;
            }

        }

        public UnisolResponse<List<UniContract>> ExpiredContracts()
        {
            var result = new UnisolResponse<List<UniContract>>();
            try
            {
                var sentContracts = _notifierDbContext.SentNotifications
                    .Where(s => s.Event.Equals(UniEvent.ExpiredContract)
                                && s.Department.Equals(Department.Hr)
                                && s.Success)
                    .Select(s => s.UniIntId)
                    .ToList();

                var contracts = _unisolDbContext.HrpContracts
                    .Join(_unisolDbContext.HrpEmployees, c => c.EmpNo, e => e.EmpNo, (c, e) => new
                    UniContract
                    {
                        Names = e.Names,
                        ID = c.ID,
                        Edate = c.Edate,
                        Sdate = c.Sdate,
                        Ref = c.Ref,
                        Notes = c.Notes,
                        EmpNo = e.EmpNo,
                        Cell = e.Cell,
                        WEmail = e.WEmail,
                        WTel = e.WTel,
                        Rdate = c.Rdate

                    })
                    .Where(n => n.Edate == DateTime.Now.AddDays(-1)
                                && !sentContracts.Contains(n.ID))
                    .ToList();

                result.Success = contracts.Count > 0;
                result.Message = $"{contracts.Count} Contracts found";
                result.Data = contracts;
                return result;
            }
            catch (Exception e)
            {
                result.Message = "Failed to get contracts";
                result.ErrorMessage = e.Message;
                return result;
            }
        }

        public UnisolResponse<List<NotificationRecipient>> GetRecipents(List<Notification> notifications)
        {
            try
            {
                var docs = notifications.Select(n => n.DocNo.ToUpper()).ToList();
                var docCentres = _unisolDbContext.WfdocCentre.Where(d => d.LatestApprovalStatus.ToLower().Equals("pending") 
                || docs.Contains(d.DocNo.ToUpper())).ToList();
                var recipients = new List<NotificationRecipient>();
                docCentres.ForEach(d =>
                {
                    var notification = notifications.FirstOrDefault(n => n.DocNo.ToUpper().Equals(d.DocNo.ToUpper()));
                    var approverResponse= GetApprover(notification.Approvers, d.Id);
                    if (approverResponse.Success)
                    {
                        var requestor = _unisolDbContext.HrpEmployees.FirstOrDefault(e => e.EmpNo.ToUpper().Equals(d.UserRef.ToUpper()));
                        var approver = _unisolDbContext.HrpEmployees.FirstOrDefault(e => e.EmpNo.ToUpper().Equals(approverResponse.Data.EmpNo.ToUpper()));
                        if (requestor != null && approver != null)
                            if (!string.IsNullOrEmpty(requestor.WEmail) && !string.IsNullOrEmpty(approver.WEmail))
                            {
                                recipients.Add(new NotificationRecipient
                                {
                                    DocNo = d.DocNo,
                                    Status = $"{notification.Status} )*( {d.LatestApprovalStatus}",
                                    IsFinalStatus = d.FinalStatus,
                                    EmpNo = d.UserRef,
                                    Approver = approver.EmpNo,
                                    EmpEmail = requestor.WEmail,
                                    ApproverEmail = approver.WEmail,
                                    ApproverTitle = approverResponse.Data?.Title,
                                    DocType = d.Type,
                                    Description = d.Description,
                                    Department = d.Department,
                                    Names = requestor.Names,
                                    ApproverLevel = approverResponse.Data.Level,
                                    ApproverUserCode = approverResponse.Data.UserCode
                                });
                            }
                    }
                    
                });
                
                return new UnisolResponse<List<NotificationRecipient>>
                {
                    Success = true,
                    Data = recipients
                };
            }
            catch (Exception ex)
            {
                return new UnisolResponse<List<NotificationRecipient>>
                {
                    Success = false
                };
            }
        }

        private UnisolResponse<Approver> GetApprover(IEnumerable<Approver> approvedApprovers, int id)
        {
            try
            {
                var arrApprovedApprovers = approvedApprovers.Select(a => a.UserCode.ToUpper()).ToList();
                var approvers = _unisolDbContext.WfdocCentreDetails.Where(d => d.Ref == $"{id}");
                var unApprovedApprovers = approvers.Where(a => !arrApprovedApprovers.Contains(a.UserCode.ToUpper()));
                var approver = unApprovedApprovers.OrderBy(a => a.Level).FirstOrDefault();

                if (approver == null)
                    return new UnisolResponse<Approver> { 
                        Success = false
                    };
                var user = _unisolDbContext.Users.FirstOrDefault(u => u.UserCode.ToUpper().Equals(approver.UserCode.ToUpper()));
                if(user == null)
                    return new UnisolResponse<Approver>
                    {
                        Success = false
                    };

                return new UnisolResponse<Approver>
                {
                    Success = true,
                    Data = new Approver
                    {
                        EmpNo = user.EmpNo,
                        Title = approver.Approver,
                        Level = (int)approver.Level,
                        UserCode  =approver.UserCode
                    }
                };
            }
            catch (Exception ex)
            {
                return new UnisolResponse<Approver>
                {
                    Success = false
                };
            }
        }

        public UnisolResponse<List<UniPendingImprest>> OverdueImprests()
        {
            var result = new UnisolResponse<List<UniPendingImprest>>();
            try
            {
                var surrenderedImprests = _unisolDbContext.ImprestSurs
                    //                    .Where(s => s.RDate >= DateTime.Today.AddDays(-1))
                    .Select(s => s.ImpRef).ToList();

                var disbImprests = _unisolDbContext.Imprests
                    .Join(_unisolDbContext.ImprestDisbs, i => i.ImpRef, d => d.ImpRef, (i, d) =>
                        new Imprest
                        {
                            Names = i.Names,
                            ImpRef = i.ImpRef,
                            Sdate = i.Sdate,
                            Rdate = i.Rdate,
                            Amount = i.Amount,
                            Description = i.Description,
                            PayeeRef = i.PayeeRef
                        })
                    .Join(_unisolDbContext.HrpEmployees, im => im.PayeeRef, e => e.EmpNo, (im, e) =>
                               new UniPendingImprest
                               {
                                   Names = e.Names,
                                   WEmail = e.WEmail,
                                   PEmail = e.PEmail,
                                   WTel = e.WTel,
                                   Cell = e.Cell,
                                   Description = im.Description,
                                   ImpRef = im.ImpRef,
                                   Sdate = im.Sdate,
                                   Amount = im.Amount,
                                   PayeeRef = im.PayeeRef,
                                   Rdate = im.Rdate
                               })
                    .Where(i => i.Sdate == DateTime.Today.AddDays(-1)
                                && !surrenderedImprests.Contains(i.ImpRef))
                    .ToList();

                var sentImprests = _notifierDbContext.SentNotifications
                    .Where(s => s.Event.Equals(UniEvent.OverdueImprests)
                                && s.Department.Equals(Department.Finance)
                                && s.Success)
                    .Select(s => s.UniStrId)
                    .ToList();

                var overdueImprests = disbImprests.
                    Where(i => !sentImprests.Contains(i.ImpRef)).ToList();

                result.Success = overdueImprests.Any();
                result.Data = overdueImprests;
                result.Message = $"{overdueImprests.Count} imprests found";

                return result;
            }
            catch (Exception e)
            {
                result.Message = "Failed to get imprests";
                result.ErrorMessage = e.Message;
                return result;
            }
        }

        public UnisolResponse<List<UniPendingImprest>> ImprestStatus(string status)
        {
            var result = new UnisolResponse<List<UniPendingImprest>>();
            try
            {
                var imprests = _unisolDbContext.Imprests
                    .Join(_unisolDbContext.HrpEmployees, im => im.PayeeRef, e => e.EmpNo, (im, e) =>
                               new UniPendingImprest
                               {
                                   Names = e.Names,
                                   WEmail = e.WEmail,
                                   PEmail = e.PEmail,
                                   WTel = e.WTel,
                                   Cell = e.Cell,
                                   Description = im.Description,
                                   ImpRef = im.ImpRef,
                                   Sdate = im.Sdate,
                                   Amount = im.Amount,
                                   PayeeRef = im.PayeeRef,
                                   Rdate = im.Rdate,
                                   Notes = im.Notes,
                                   Status = im.Status,
                                   Itinerary = im.Itinerary
                               })
                    .Where(i => i.Status.Equals(status))
                    .ToList();

                var sentImprests = _notifierDbContext.SentNotifications
                    .Where(s => s.Event.Equals(UniEvent.ApprovedImprests)
                                && s.Department.Equals(Department.Finance)
                                && s.Success)
                    .Select(s => s.UniStrId)
                    .ToList();

                var approvedImprests = imprests.
                    Where(i => !sentImprests.Contains(i.ImpRef)).ToList();

                result.Success = approvedImprests.Any();
                result.Data = approvedImprests;
                result.Message = $"{approvedImprests.Count} {status} imprests found";

                return result;
            }
            catch (Exception e)
            {
                result.Message = $"Failed to get {status} imprests";
                result.ErrorMessage = e.Message;
                return result;
            }
        }

        public UnisolResponse<List<UniPendingImprest>> DisbursedImprests()
        {
            var result = new UnisolResponse<List<UniPendingImprest>>();
            try
            {
                var disbImprests = _unisolDbContext.Imprests
                    .Join(_unisolDbContext.ImprestDisbs, i => i.ImpRef, d => d.ImpRef, (i, d) =>
                        new Imprest
                        {
                            Names = i.Names,
                            ImpRef = i.ImpRef,
                            Sdate = i.Sdate,
                            Rdate = d.RDate,
                            Amount = i.Amount,
                            Description = i.Description,
                            PayeeRef = i.PayeeRef
                        })
                    .Join(_unisolDbContext.HrpEmployees, im => im.PayeeRef, e => e.EmpNo, (im, e) =>
                               new UniPendingImprest
                               {
                                   Names = e.Names,
                                   WEmail = e.WEmail,
                                   PEmail = e.PEmail,
                                   WTel = e.WTel,
                                   Cell = e.Cell,
                                   Description = im.Description,
                                   ImpRef = im.ImpRef,
                                   Sdate = im.Sdate,
                                   Amount = im.Amount,
                                   PayeeRef = im.PayeeRef,
                                   Rdate = im.Rdate
                               })
                    .Where(i => i.Rdate == DateTime.Today)
                    .ToList();

                var sentImprests = _notifierDbContext.SentNotifications
                    .Where(s => s.Event.Equals(UniEvent.ApprovedImprests)
                                && s.Department.Equals(Department.Finance)
                                && s.Success)
                    .Select(s => s.UniStrId)
                    .ToList();

                var approvedImprests = disbImprests.
                    Where(i => !sentImprests.Contains(i.ImpRef)).ToList();

                result.Success = approvedImprests.Any();
                result.Data = approvedImprests;
                result.Message = $"{approvedImprests.Count} imprests found";

                return result;
            }
            catch (Exception e)
            {
                result.Message = "Failed to get imprests";
                result.ErrorMessage = e.Message;
                return result;
            }
        }

        public UnisolResponse<List<UniLeaveApp>> LeaveStatus(string status)
        {
            var result = new UnisolResponse<List<UniLeaveApp>>();
            try
            {
                var leaveApps = _unisolDbContext.HrLeaveApps
                    .Join(_unisolDbContext.HrpEmployees, ap => ap.EmpNo, e => e.EmpNo, (ap, e) =>
                        new UniLeaveApp
                        {
                            Names = e.Names,
                            WEmail = e.WEmail,
                            PEmail = e.PEmail,
                            WTel = e.WTel,
                            Cell = e.Cell,
                            EmpNo = e.EmpNo,
                            Ref = ap.Ref,
                            LeaveType = ap.LeaveType,
                            Sdate = ap.Sdate,
                            Edate = ap.Edate,
                            Stime = ap.Stime,
                            Etime = ap.Etime,
                            LeaveDays = ap.LeaveDays,
                            LeavePeriod = ap.LeavePeriod,
                            Status = ap.Status,
                            Rdate = ap.Rdate,
                            Notes = ap.Notes
                        })
                    .Where(i => i.Status.Equals(status))
                    .ToList();

                var sentLeaves = _notifierDbContext.SentNotifications
                    .Where(s => s.Event.Equals(UniEvent.LeaveStatus)
                                && s.Department.Equals(Department.Hr)
                                && s.Success)
                    .Select(s => s.UniStrId)
                    .ToList();

                var notSent = leaveApps.
                    Where(i => !sentLeaves.Contains(i.Ref))
                    .ToList();

                result.Success = notSent.Any();
                result.Data = notSent;
                result.Message = $"{notSent.Count} leave applications found";

                return result;

            }
            catch (Exception e)
            {
                result.Message = "Failed to get leave applications";
                result.ErrorMessage = e.Message;
                return result;
            }
        }

    }
}