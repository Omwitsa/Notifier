using AbnNotifier.Transfer;
using System;
using System.Collections.Generic;

namespace AbnNotifier.Data.Notifier.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string DocNo { get; set; }
        public string Status { get; set; }
        public bool IsFinalStatus { get; set; }
        public string Content { get; set; }
        public string Department { get; set; }
        public string Empno { get; set; }
        public IEnumerable<Approver> Approvers { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
