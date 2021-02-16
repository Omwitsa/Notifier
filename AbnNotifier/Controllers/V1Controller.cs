using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbnNotifier.Data.Notifier;
using AbnNotifier.Data.Notifier.Models;
using AbnNotifier.Services;
using AbnNotifier.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Unisol.Web.Common.Utilities;

namespace AbnNotifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V1Controller : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly AbnNotifierDbContext _context;
        private readonly string _conStr;
        private readonly AbnNotifierService _notifierService;
        public V1Controller(IConfiguration configuration, AbnNotifierDbContext context)
        {
            Configuration = configuration;
            _context = context;
            _conStr = DbSetting.ConnectionString(Configuration, "Unisol");
			_notifierService = new AbnNotifierService(_context, _conStr, false);
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public string SendNotification()
        {
            _notifierService.SendNotification();
            return "";
        }

        [HttpGet("[action]")]
        public async Task<string> Setup()
        {
            var con = DbSetting.ConnectionString(Configuration, "Unisol");
			var service = new AbnNotifierService(_context, con, true);
            //service.SendNotification();

            var email = _context.EmailSmsSettings.FirstOrDefault(e => e.Key.Equals(SettingKey.Email));
            if (email == null)
            {
                var smtpServer = Configuration["Email:smtpServer"];
                var smtpPort = Configuration["Email:smtpPort"];
                var smtpUsername = Configuration["Email:smtpUsername"];
                var smtpPassword = Configuration["Email:smtpPassword"];
                var senderFromEmail = Configuration["Email:senderFromEmail"];
                var senderFromName = Configuration["Email:senderFromName"];

                var emailSettings = new EmailSetting
                {
                    SmtpPassword = smtpPassword,
                    SmtpPort = smtpPort,
                    SmtpServer = smtpServer,
                    SmtpUsername = smtpUsername,
                    SenderFromEmail = senderFromEmail,
                    SenderFromName = senderFromName
                };

                var data = JsonConvert.SerializeObject(emailSettings);
                var setting = new EmailSmsSetting
                {
                    Key = SettingKey.Email,
                    Data = data,
                    CanSend = true
                };
                _context.EmailSmsSettings.Add(setting);
                _context.SaveChanges();

                return "Hello World";
            }

            email.CanSend = true;
            _context.SaveChanges();

            return "Hello World";

        }
    }
}
