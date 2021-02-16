using System;

namespace AbnNotifier.Data.Notifier.Models
{
    public class Approver
    {
        public Guid Id { get; set; }
        public Guid? NotificationId { get; set; }
        public string EmpNo { get; set; }
        public string UserCode { get; set; }
        public int Level { get; set; }
        public string Title { get; set; }
    }
}
