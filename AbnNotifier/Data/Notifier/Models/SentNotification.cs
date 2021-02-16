using System;

namespace AbnNotifier.Data.Notifier.Models
{
    public class SentNotification : BaseEntity
    {
        public SentNotification()
        {
            Code = "SENT-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
            Success = false;
        }
        public int UniIntId { get; set; }
        public string UniStrId { get; set; }
        public string EmpNo { get; set; }
        public string Supervisor { get; set; }
        public Department Department { get; set; }
        public UniEvent Event { get; set; }
        public string Content { get; set; }
        public string Response { get; set; }
        public bool Success { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public enum UniEvent
    {
        ExpiredContract,
        OverdueImprests,
        ApprovedImprests,
        LeaveStatus
    }
}
