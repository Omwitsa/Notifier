using AbnNotifier.Data.Notifier.Models;

namespace AbnNotifier.Transfer
{
    public class AddSupervisorRequest
    {
        public string EmpNo { get; set; }
        public Department Department { get; set; }
    }
}
