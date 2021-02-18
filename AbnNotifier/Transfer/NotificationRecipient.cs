using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbnNotifier.Transfer
{
    public class NotificationRecipient
    {
        public string DocNo { get; set; }
        public string Status { get; set; }
        public string IsFinalStatus { get; set; }
        public string EmpNo { get; set; }
        public string Approver { get; set; }
        public string EmpEmail { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverTitle { get; set; }
        public string DocType { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Names { get; set; }
        public string ApproverUserCode { get; set; }
        public int ApproverLevel { get; set; }
        public string ApproverNames { get; set; }
    }
}
