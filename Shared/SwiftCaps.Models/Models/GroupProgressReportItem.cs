using System;
using SwiftCaps.Models.Enums;

namespace SwiftCaps.Models.Models
{
    public class GroupProgressReportItem
    {
        public string GroupName { get; set; }
        public string QuizName { get; set; }
        public Recurrence? Recurrence { get; set; } = null;
        public string Sequence { get; set; }
        public int? DonePercentage { get; set; } = null;
        public string AvergageTime { get; set; }
        public int UserCount { get; set; }
    }
}
