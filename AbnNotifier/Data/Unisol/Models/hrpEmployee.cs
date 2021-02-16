using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("hrpEmployee")]
    public class hrpEmployee
    {
        [Key]
        public string EmpNo { get; set; }
        public string Names { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string IDNo { get; set; }
        public DateTime HDate { get; set; }
        public string Cell { get; set; }
        public string PEmail { get; set; }
        public string WTel { get; set; }
        public string WEmail { get; set; }
        public string Supervisor { get; set; }
        public bool Terminated { get; set; }
    }
}
