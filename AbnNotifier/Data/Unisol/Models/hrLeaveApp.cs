﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbnNotifier.Data.Unisol.Models
{
    [Table("hrpLeaveApp")]
    public class hrLeaveApp
    {
        [Key]
        public string Ref { get; set; }
        public string LeavePeriod { get; set; }
        public string EmpNo { get; set; }
        public string LeaveType { get; set; }
        public DateTime Sdate { get; set; }
        public string Stime { get; set; }
        public DateTime Edate { get; set; }
        public string Etime { get; set; }
        public decimal LeaveDays { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime Rdate { get; set; }
        public string Personnel { get; set; }
    }
}
