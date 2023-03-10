using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class GroupProgressReportItemViewModel
    {
        public string GroupName { get; set; }
        public string QuizName { get; set; }
        public Recurrence? Recurrence { get; set; } = null;
        public string RecurrenceDisplayText => (Recurrence != null) ? Recurrence.ToString() : "";
        public string Sequence { get; set; }
        public int? DonePercentage { get; set; } = null;
        public string DonePercentageFormat => (!string.IsNullOrEmpty(QuizName) ? $"{DonePercentage ?? 0}%" : "");
        public string AvergageTime { get; set; }
        public int UserCount { get; set; }
    }
}
