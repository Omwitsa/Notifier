using AbnNotifier.Data.Unisol.Models;

namespace AbnNotifier.Transfer.Unisol
{
    public class UniEmployee
    {
        public UniEmployee(hrpEmployee employee)
        {
            EmpNo = employee.EmpNo;
            Names = employee.Names;
            WEmail = employee.WEmail;
            PEmail = employee.PEmail;
            WTel = employee.WTel;
            Cell = employee.Cell;

        }

        public UniEmployee()
        {

        }
        public string EmpNo { get; set; }
        public string Names { get; set; }
        public string WEmail { get; set; }
        public string PEmail { get; set; }
        public string WTel { get; set; }
        public string Cell { get; set; }
    }
}
