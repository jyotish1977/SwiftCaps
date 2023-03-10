using System;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class QuizSummaryItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Questions { get; set; }
        public int Sections { get; set; }
        public string LastUpdated { get; set; }
        public string Groups { get; set; }
        public int Schedules { get; set; }
    }
}
