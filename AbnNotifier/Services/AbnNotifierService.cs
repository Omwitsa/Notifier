using System;
using System.Collections.Generic;
using System.Linq;
using AbnNotifier.Data.Notifier;
using AbnNotifier.Data.Notifier.Models;
using AbnNotifier.Services.Email;
using AbnNotifier.Transfer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AbnNotifier.Services
{
    public class AbnNotifierService
    {
        private readonly AbnNotifierDbContext _context;
        private readonly UnisolService _unisolService;
        private readonly EmailService _emailService;
        private readonly bool _isSetup;
        public AbnNotifierService(AbnNotifierDbContext context, string unisolConnection, bool isSetup)
        {
            _context = context;
            _unisolService = new UnisolService(unisolConnection, _context);
            _emailService = new EmailService();
            _isSetup = isSetup;
        }

        public void SendDisbursedImprests()
        {
            try
            {
                var imprests = _unisolService.DisbursedImprests();
                if (!imprests.Success)
                {
                    return;
                }

                if (imprests.Data.Count > 0)
                {
                    var notSentImprests = imprests.Data.Take(1).ToList();

                    foreach (var imprest in notSentImprests)
                    {
                        var message =
                            $"Employee No. {imprest.PayeeRef} :  Imprest of Ref {imprest.ImpRef} - {imprest.Description}  of amount {imprest.Amount} has been Disbursed. Surrender by {imprest.Sdate:D}.";

                        var notification = new SentNotification
                        {
                            EmpNo = imprest.PayeeRef,
                            UniStrId = imprest.ImpRef,
                            Content = message,
                            Department = Department.Finance,
                            Event = UniEvent.ApprovedImprests
                        };
                        if (!string.IsNullOrEmpty(imprest.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(imprest.Names, imprest.WEmail, "Approved Imprest", message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;

                            notification.Email = imprest.WEmail;
                            notification.Phone = imprest.WTel;
                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response = "Employee does not have a working email";
                            AddNotification(notification);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendNotification()
        {
            try
            {
                var notifications = _context.Notification.Where(n => !n.IsFinalStatus).Select(n => new Notification { 
                    DocNo  = n.DocNo,
                    Status = n.Status
                }).Include(n => n.Approvers).ToList(); 

                var recipientResponse = _unisolService.GetRecipents(notifications);
                if (!recipientResponse.Success)
                    return;

                recipientResponse.Data.ForEach(n =>
                {
                    var message = $"Employee No. ({n.EmpNo}) : Doc Type. ({n.DocType}) : Decription. {n.Description} : Approver . {n.ApproverTitle}";
                    var notification = _context.Notification.FirstOrDefault(t => t.DocNo.ToUpper().Equals(n.DocNo.ToUpper()));
                    var approvers = new List<Approver>();
                    if (notification != null)
                    {
                        approvers = _context.Approver.Where(a => a.NotificationId == notification.Id).ToList();
                        _context.Approver.RemoveRange(approvers);
                        _context.Notification.Remove(notification);
                    }
                    approvers.Add(new Approver
                    {
                        EmpNo = n.Approver,
                        UserCode = n.ApproverUserCode,
                        Level = n.ApproverLevel,
                        Title = n.ApproverTitle
                    });

                    notification = new Notification
                    {
                        DocNo = n.DocNo,
                        Status = n.Status,
                        Content = message,
                        Department = n.Department,
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        DateModified = DateTime.UtcNow.AddHours(3),
                        Empno = n.EmpNo,
                        Approvers = approvers
                    };

                    var sent = new EmailResponse { Sent = true, Message = "Test" };
                    if (!_isSetup)
                    {
                        sent = SendEmail(n.Names, n.EmpEmail, n.DocType, message);
                        sent = SendEmail(n.Names, n.ApproverEmail, n.DocType, message);
                    }

                    _context.SaveChanges();
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void SendOverdueImprests()
        {
            try
            {
                var imprests = _unisolService.OverdueImprests();
                if (!imprests.Success)
                {
                    return;
                }

                if (imprests.Data.Count > 0)
                {
                    var notSentImprests = imprests.Data.Take(1).ToList();

                    var fmEmpNo = _context.Supervisors
                        .FirstOrDefault(s => s.Department.Equals(Department.Finance))?.EmpNo;

                    var fm = _unisolService.GetByEmpNo(fmEmpNo).Data;
                    foreach (var imprest in notSentImprests)
                    {
                        var message =
                            $"Employee No. {imprest.PayeeRef} :  Imprest of Ref {imprest.ImpRef} - {imprest.Description} was to surrendered by {imprest.Sdate:D}.";

                        var notification = new SentNotification
                        {
                            EmpNo = imprest.PayeeRef,
                            UniStrId = imprest.ImpRef,
                            Content = message,
                            Department = Department.Finance,
                            Event = UniEvent.OverdueImprests,
                            Supervisor = fmEmpNo
                        };
                        if (!string.IsNullOrEmpty(imprest.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(imprest.Names, imprest.WEmail, "Overdue Imprest", message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;

                            notification.Email = imprest.WEmail;
                            notification.Phone = imprest.WTel;
                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response = "Employee does not have a working email";
                            AddNotification(notification);
                        }

                        if (!string.IsNullOrEmpty(fm.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(fm.Names, fm.WEmail, "Overdue Imprest",
                                  message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;

                            notification.Email = fm.WEmail;
                            notification.Phone = fm.WTel;
                            AddNotification(notification);

                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response =
                                $"Employee supervisor in {Department.Finance} Department does not have a working email";

                            AddNotification(notification);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendExpiredContracts()
        {
            try
            {
                var contracts = _unisolService.ExpiredContracts();
                if (!contracts.Success)
                {
                    return;
                }

                if (contracts.Data.Count > 0)
                {
                    var notSentContracts = contracts.Data;

                    var hrEmpNo = _context.Supervisors
                        .FirstOrDefault(s => s.Department.Equals(Department.Hr))?.EmpNo;
                    var hr = _unisolService.GetByEmpNo(hrEmpNo).Data;

                    foreach (var contract in notSentContracts)
                    {
                        var message =
                            $"Employee No. {contract.EmpNo} : The contract Ref {contract.Ref}-{contract.Notes} with start date {contract.Sdate:D} to {contract.Edate:D} has Expired.";

                        var notification = new SentNotification
                        {
                            EmpNo = contract.EmpNo,
                            Content = message,
                            UniIntId = contract.ID,
                            Department = Department.Hr,
                            Event = UniEvent.ExpiredContract,
                            Supervisor = hrEmpNo
                        };
                        if (!string.IsNullOrEmpty(contract.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(contract.Names, contract.WEmail, "Lapse of Contract", message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;

                            notification.Email = contract.WEmail;
                            notification.Phone = contract.WTel;
                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response = "Employee does not have a working email";
                            AddNotification(notification);
                        }

                        if (!string.IsNullOrEmpty(hr?.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(hr.Names, hr.WEmail, "Lapse of Contract",
                                  message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;

                            notification.Email = hr.WEmail;
                            notification.Phone = hr.WTel;
                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response =
                                $"Employee supervisor in {Department.Hr} Department does not have a working email";

                            AddNotification(notification);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendImprestStatus(string status)
        {
            try
            {
                var apps = _unisolService.ImprestStatus(status);
                if (!apps.Success)
                {
                    return;
                }

                if (apps.Data.Count > 0)
                {
                    var notSentImprests = apps.Data;

                    var fmEmpNo = _context.Supervisors
                        .FirstOrDefault(s => s.Department.Equals(Department.Finance))?.EmpNo;
                    var fm = _unisolService.GetByEmpNo(fmEmpNo).Data;

                    foreach (var imprest in notSentImprests)
                    {
                        var message =
                            $"Employee No. {imprest.EmpNo} : The imprest {imprest.ImpRef}-{imprest.Itinerary}  has been {imprest.Status}. {imprest.Notes} ";

                        var notification = new SentNotification
                        {
                            EmpNo = imprest.EmpNo,
                            Content = message,
                            UniStrId = imprest.ImpRef,
                            Department = Department.Finance,
                            Event = UniEvent.ApprovedImprests,
                            Supervisor = fmEmpNo
                        };
                        if (!string.IsNullOrEmpty(imprest.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(imprest.Names, imprest.WEmail, $"Imprest Status -{imprest.Status}", message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;
                            notification.Email = imprest.WEmail;
                            notification.Phone = imprest.WTel;

                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response = "Employee does not have a working email";
                            notification.Email = imprest?.WEmail;
                            notification.Phone = imprest?.WTel;

                            AddNotification(notification);
                        }

                        if (!string.IsNullOrEmpty(fm?.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(fm.Names, fm.WEmail, $"Imprest Status -{imprest.Status}",
                                  message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;
                            notification.Email = fm.WEmail;
                            notification.Phone = fm.WTel;

                            AddNotification(notification);

                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response =
                                $"Employee supervisor in {Department.Finance} Department does not have a working email";
                            notification.Email = fm?.WEmail;
                            notification.Phone = fm?.WTel;

                            AddNotification(notification);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void SendLeaveStatus(string status)
        {
            try
            {
                var apps = _unisolService.LeaveStatus(status);
                if (!apps.Success)
                {
                    return;
                }

                if (apps.Data.Count > 0)
                {
                    var notSentApps = apps.Data;

                    var hrEmpNo = _context.Supervisors
                        .FirstOrDefault(s => s.Department.Equals(Department.Hr))?.EmpNo;
                    var hr = _unisolService.GetByEmpNo(hrEmpNo).Data;

                    foreach (var app in notSentApps)
                    {
                        var message =
                            $"Employee No. {app.EmpNo} : The leave application {app.Ref}-{app.LeaveType} ({app.LeaveDays} days) with " +
                            $"start date {app.Sdate:D} to {app.Edate:D} is {app.Status}. Notes : {app.Notes}";

                        var notification = new SentNotification
                        {
                            EmpNo = app.EmpNo,
                            Content = message,
                            UniStrId = app.Ref,
                            Department = Department.Hr,
                            Event = UniEvent.LeaveStatus,
                            Supervisor = hrEmpNo
                        };
                        if (!string.IsNullOrEmpty(app.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(app.Names, app.WEmail, $"Leave Status -{app.Status}", message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;
                            notification.Email = app.WEmail;
                            notification.Phone = app.WTel;

                            AddNotification(notification);
                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response = "Employee does not have a working email";
                            notification.Email = app?.WEmail;
                            notification.Phone = app?.WTel;

                            AddNotification(notification);
                        }

                        if (!string.IsNullOrEmpty(hr?.WEmail))
                        {
                            var sent = new EmailResponse { Sent = true, Message = "Test" };
                            if (!_isSetup)
                                sent = SendEmail(hr.Names, hr.WEmail, $"Leave Status -{app.Status}",
                                  message);
                            notification.Success = sent.Sent;
                            notification.Response = sent.Message;
                            notification.Email = hr.WEmail;
                            notification.Phone = hr.WTel;

                            AddNotification(notification);

                        }
                        else
                        {
                            notification.Success = false;
                            notification.Response =
                                $"Employee supervisor in {Department.Hr} Department does not have a working email";
                            notification.Email = hr?.WEmail;
                            notification.Phone = hr?.WTel;

                            AddNotification(notification);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void AddNotification(SentNotification sent)
        {
            sent.Id = Guid.NewGuid();
            _context.SentNotifications.Add(sent);
            _context.SaveChanges();
        }

        private EmailResponse SendEmail(string name, string email, string subject, string content)
        {
            var response = new EmailResponse { Message = "Not Sent" };
            try
            {
                var emailSetting = _context.EmailSmsSettings.FirstOrDefault(s => s.Key.Equals(SettingKey.Email));
                if (emailSetting != null)
                {
                    if (!emailSetting.CanSend)
                    {
                        response.Sent = true;
                        response.Message = "Email setup not run";
                        return response;
                    }
                    var settings = JsonConvert.DeserializeObject<EmailSetting>(emailSetting?.Data);
                    var emailAddress = new EmailAddress
                    {
                        Name = name,
                        Address = email
                    };
                    var address = new EmailAddress
                    {
                        Name = string.IsNullOrEmpty(settings.SenderFromName)
                            ? "ABNO Softwares International Ltd"
                            : settings.SenderFromName,
                        Address = string.IsNullOrEmpty(settings.SenderFromEmail)
                            ? "devs@abnosoftwares.co.ke"
                            : settings.SenderFromEmail
                    };

                    var logoImageUrl = settings.LogoUrl;

                    var emailMessage = new EmailMessage
                    {
                        ToAddresses = new List<EmailAddress> { emailAddress },
                        Content = Template.Get(subject, name, content),
                        Subject = subject,
                        FromAddresses = new List<EmailAddress> { address },
                        Logo = logoImageUrl
                    };

                    var rSend = _emailService.Send(emailMessage, settings);
                    var msg = rSend.Message;
                    response.Message = rSend.Sent ? $"OK : {msg}" : $"Not sent : {msg}";
                    response.Sent = rSend.Sent;
                    return response;
                }

                return response;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;

            }
        }
    }
}
