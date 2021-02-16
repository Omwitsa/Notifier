using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("hrpContract")]
    public class hrpContract
    {
        public int ID { get; set; }
        public string EmpNo { get; set; }
        public string Ref { get; set; }
        public DateTime Sdate { get; set; }
        public DateTime Edate { get; set; }
        public DateTime Rdate { get; set; }
        public string Notes { get; set; }
    }
}
