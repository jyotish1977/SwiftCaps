using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SwiftCAPS.Admin.Web.Shared.Models
{
    public class QuizSectionViewModel
    {
        [Required (ErrorMessage = "Name is required.")]
        public string Description { get; set; }
    }
}
