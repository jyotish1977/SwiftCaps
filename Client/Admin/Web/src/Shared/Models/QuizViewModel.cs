using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class QuizViewModel
    {
        [Required (ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string InfoMarkdown { get; set; }
    }
}
