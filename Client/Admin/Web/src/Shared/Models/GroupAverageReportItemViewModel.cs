using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class GroupAverageReportItemViewModel
    {
        public string GroupName { get; set; }
        public int QuizCount { get; set; }
        public int? AverageDonePercentage { get; set; } = null;
        public string AverageDonePercentageFormat => (QuizCount > 0 ? $"{AverageDonePercentage ?? 0}%" : "");
        public string AvergageTime { get; set; }
        public int UserCount { get; set; }
    }
}
