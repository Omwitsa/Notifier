using System;

namespace AbnNotifier.Transfer.Unisol
{
    public class UniContract : UniEmployee
    {
        public int ID { get; set; }
        public string Ref { get; set; }
        public DateTime Edate { get; set; }
        public DateTime Sdate { get; set; }
        public DateTime Rdate { get; set; }
        public string Notes { get; set; } 

    }
}
