using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("Imprest")]
    public class Imprest
    {
        [Key]
        public string ImpRef { get; set; }
        public string PayeeRef { get; set; }
        public string Names { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Sdate { get; set; }
        public DateTime Rdate { get; set; }
        public string Itinerary { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }

    }
}
