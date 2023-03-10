using System;
using System.ComponentModel.DataAnnotations;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class QuestionViewModel
    {
        public string Description { get; set; }

        [Required(ErrorMessage = "Body is required.")]
        public string Body { get; set; }

        public Guid QuizSectionId { get; set; }

        public int QuizSectionIndex { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }
    }
}
