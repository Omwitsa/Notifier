using System;

namespace AbnNotifier.Data.Notifier.Models
{
    public class Supervisor : BaseEntity
    {
        public Supervisor()
        {
            Code = "SUP-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
        }
        public string EmpNo { get; set; }
        public Department Department { get; set; }
        public string DepartmentStr => Department.ToString();
    }

    public enum Department
    {
        Hr,
        Finance,
        Procurement,
        Assets
    }
}
