using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SwiftCaps.Models.Enums;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class ScheduleViewModel
    {
        [Required(ErrorMessage = "Quiz is required.")]
        public Guid QuizId { get; set; }

        [Required(ErrorMessage = "Recurrence is required.")]
        public Recurrence Recurrence { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime? StartTime { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End time is required.")]
        public DateTime? EndTime { get; set; } = DateTime.Now;
    }
}
