using System;
using System.Linq;
using AbnNotifier.Transfer;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace AbnNotifier.Services.Email
{
    public class EmailService
    {
        public EmailResponse Send(EmailMessage emailMessage, EmailSetting settings)
        {
            settings.SmtpServer = settings?.SmtpServer ?? "";
            settings.SmtpPort = settings?.SmtpPort ?? "";
            settings.SmtpUsername = settings?.SmtpUsername ?? "";
            settings.SmtpPassword = settings?.SmtpPassword ?? "";
            var response = new EmailResponse();
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.Subject = emailMessage.Subject;

            var builder = new BodyBuilder { HtmlBody = emailMessage.Content };

            message.Body = builder.ToMessageBody();

            using (var emailClient = new SmtpClient())
            {
                var smtpServer = settings.SmtpServer;
                var smtpPort = int.Parse(settings.SmtpPort);
                var smtpUsername = settings.SmtpUsername;
                var smtpPassword = settings.SmtpPassword;

                var options = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                try
                {
                    emailClient.Connect(smtpServer, smtpPort, options);
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                    emailClient.Authenticate(smtpUsername, smtpPassword);
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    return response;
                }
                emailClient.Send(message);

                emailClient.Disconnect(true);
            }

            response.Sent = true;
            response.Message = "Sent";
            return response;
        }

    }
}