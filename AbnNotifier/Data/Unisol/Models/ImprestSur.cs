using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("ImprestSur")]
    public class ImprestSur
    {
        [Key]
        public string ImpRef { get; set; }
        public string Personnel { get; set; }
        public DateTime RDate { get; set; }
        public string Notes { get; set; }
        public decimal ActualAmount { get; set; }

    }
}
