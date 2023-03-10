using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class QuizAverageReportItemViewModel
    {
        public string QuizName { get; set; }
        public int? DonePercentage { get; set; } = null;
        public string DonePercentageFormat => (!string.IsNullOrEmpty(QuizName) ? $"{DonePercentage ?? 0}%" : "");
        public string AvergageTime { get; set; }
        public int GroupCount { get; set; }
        public int UserCount { get; set; }
    }
}
