using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("ImprestDisb")]
    public class ImprestDisb
    {
        [Key]
        public string ImpRef { get; set; }
        public DateTime RDate { get; set; }
        public string Personnel { get; set; }
        public string Notes { get; set; }
    }
}
