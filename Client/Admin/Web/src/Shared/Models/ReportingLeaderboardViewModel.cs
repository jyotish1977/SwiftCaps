using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class ReportingLeaderboardViewModel
    {
        public Guid UserId { get; set; }
        public string User { get; set; }
        public bool MonthlyCompleted { get; set; }
        public string MonthlyCompletedDisplayText => (MonthlyCompleted ? "100%" : "0%");
        public bool WeeklyCompleted { get; set; }
        public string WeeklyCompletedDisplayText => (WeeklyCompleted ? "100%" : "0%");
    }
}
