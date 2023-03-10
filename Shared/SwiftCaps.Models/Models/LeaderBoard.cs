using System;
using System.Collections.Generic;

namespace SwiftCaps.Models.Models
{
    public class LeaderBoard
    {
        public virtual Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid GroupId { get; set; }
        public virtual IList<QuizReport> QuizReports { get; set; }

        public virtual IList<QuizReport> WeeklyQuizReports { get; set; }
        public virtual IList<QuizReport> MonthlyQuizReports { get; set; }

    }
}
