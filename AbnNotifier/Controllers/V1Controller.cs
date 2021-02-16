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

        public string SendExpiredContracts()
        {
            _notifierService.SendExpiredContracts();
            return "";
        }

        public string SendOverdueImprests()
        {
            _notifierService.SendOverdueImprests();
            return "";
        }
        public string SendDisbursedImprests()
        {
            _notifierService.SendDisbursedImprests();
            return "";
        }
        public string SendApprovedLeaves()
        {
            _notifierService.SendLeaveStatus("Approved");
            return "";
        }

        public string SendDeclinedLeaves()
        {
            _notifierService.SendLeaveStatus("Declined");
            return "";
        }

        public string SendApprovedImprests()
        {
            _notifierService.SendImprestStatus("Approved");
            return "";
        }
        public string SendDeclinedImprests()
        {
            _notifierService.SendImprestStatus("Declined");
            return "";
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

            if (!_context.Supervisors.Any())
            {
                var hr = new Supervisor
                {
                    EmpNo = Configuration["HrEmpNo"],
                    Department = Department.Hr
                };
                var fm = new Supervisor
                {
                    EmpNo = Configuration["FmEmpNo"],
                    Department = Department.Finance
                };

                _context.Supervisors.Add(hr);
                _context.Supervisors.Add(fm);
                _context.SaveChanges();
            }


            //service.SendExpiredContracts();
            //service.SendOverdueImprests();
            //service.SendDisbursedImprests();
            //service.SendLeaveStatus("Approved");
            //service.SendLeaveStatus("Declined");
            //service.SendImprestStatus("Approved");
            //service.SendImprestStatus("Declined
            service.SendNotification();


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

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)

        {
            switch (id)
            {
                case 1:
                    _notifierService.SendExpiredContracts();
                    break;
                case 2:
                    _notifierService.SendOverdueImprests();
                    break;
                case 3:
                    _notifierService.SendDisbursedImprests();
                    break;
                case 4:
                    _notifierService.SendLeaveStatus("Approved");
                    break;
                case 5:
                    _notifierService.SendLeaveStatus("Declined");
                    break;
                case 6:
                    _notifierService.SendImprestStatus("Approved");
                    break;
                case 7:
                    _notifierService.SendImprestStatus("Declined");
                    break;
            }

            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("[action]")]
        public string SupervisorAdd([FromBody] AddSupervisorRequest request)
        {
            try
            {
                var supervisor = new Supervisor
                {
                    EmpNo = request.EmpNo,
                    Department = request.Department
                };
                _context.Supervisors.Add(supervisor);
                _context.SaveChanges();
                return $"Employee {request.EmpNo} Added to {request.Department} Department";
            }
            catch (Exception e)
            {
                return $"Employee {request.EmpNo} not Added. Error {e.Message}";
            }
        }

        [HttpGet("[action]")]
        public List<Supervisor> Supervisors()
        {
            var supervisors = _context.Supervisors.ToList();
            return supervisors;
        }
    }
}
