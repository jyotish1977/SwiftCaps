using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SwiftCaps.Models.Models
{
    public class Question : ModelBase, INotifyPropertyChanged
    {
        // Section the question belongs to
        public Guid QuizSectionId { get; set; }

        // order within a section
        public int QuizSectionIndex { get; set; }
        
        // information displayed on top of a question
        public string Header { get; set; }
        public bool HasHeader => !string.IsNullOrEmpty(Header);

        // information displayed at the bottom of a question
        public string Footer { get; set; }
        public bool HasFooter => !string.IsNullOrEmpty(Footer);

        // The question / response body
        public string Body { get; set; }

        public string Description { get; set; }

        public bool HasDescription => !string.IsNullOrEmpty(Description);

        public virtual IList<QuizAnswer> QuizAnswers { get; set; }
    }
}
