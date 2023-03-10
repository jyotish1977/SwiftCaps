using System;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class ScheduleSummaryItem
    {
        public Guid Id { get; set; }
        public string QuizName { get; set; }
        public Recurrence Recurrence { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int GroupCount { get; set; }
    }
}
