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

        public void SendNotification()
        {
            try
            { 
                var notifications = _context.Notification.Where(n => !n.IsFinalStatus).Include(n => n.Approvers).ToList(); 
                var recipientResponse = _unisolService.GetRecipents(notifications);
                if (!recipientResponse.Success)
                    return;

                foreach(var recipient in recipientResponse.Data)
                {
                    recipient.IsFinalStatus = recipient?.IsFinalStatus ?? "";
                    var message = $"Employee No. ({recipient.EmpNo}) : Doc Type. ({recipient.DocType}) : Approver . {recipient.ApproverTitle} : Status . {recipient.Status} : Decription. {recipient.Description}";
                    var notification = _context.Notification.FirstOrDefault(t => t.DocNo.ToUpper().Equals(recipient.DocNo.ToUpper()));
                    var approvers = new List<Approver>();
                    if (notification != null)
                    {
                        approvers = _context.Approver.Where(a => a.NotificationId == notification.Id).ToList();
                        _context.Approver.RemoveRange(approvers);
                        _context.Notification.Remove(notification);
                    }
                    approvers.Add(new Approver
                    {
                        Id = Guid.NewGuid(),
                        EmpNo = recipient.Approver,
                        UserCode = recipient.ApproverUserCode,
                        Level = recipient.ApproverLevel,
                        Title = recipient.ApproverTitle
                    });

                    var isPending = recipient.IsFinalStatus.ToLower().Equals("pending") || string.IsNullOrEmpty(recipient.IsFinalStatus);
                    notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        DocNo = recipient.DocNo,
                        Status = recipient.IsFinalStatus,
                        IsFinalStatus = !isPending,
                        Content = message,
                        Department = recipient.Department,
                        DateCreated = notification.DateCreated,
                        DateModified = DateTime.UtcNow.AddHours(3),
                        Empno = recipient.EmpNo, 
                        Approvers = approvers
                    };

                    var sent = new EmailResponse { Sent = true, Message = "Test" };
                    if (!_isSetup)
                    {
                        sent = SendEmail(recipient.Names, recipient.EmpEmail, recipient.DocType, message);
                        sent = SendEmail(recipient.ApproverNames, recipient.ApproverEmail, recipient.DocType, message);
                    }

                    _context.Notification.Add(notification);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
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
