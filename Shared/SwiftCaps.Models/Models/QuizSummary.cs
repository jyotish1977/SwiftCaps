using System;
namespace SwiftCaps.Models.Models
{
    public class QuizSummary
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Questions { get; set; }
        public int Sections { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int Groups { get; set; }
        public int Schedules { get; set; }
    }
}
