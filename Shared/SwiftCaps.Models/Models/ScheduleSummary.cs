using System;
using SwiftCaps.Models.Enums;

namespace SwiftCaps.Models.Models
{
    public class ScheduleSummary 
    {
        public Guid Id { get; set; }
        public string QuizName { get; set; }
        public Recurrence Recurrence { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int GroupCount { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
