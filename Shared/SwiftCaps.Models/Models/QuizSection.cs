using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SwiftCaps.Models.Models
{
    public class QuizSection : ModelBase, INotifyPropertyChanged
    {
        // All questions are contained in their respective section

        public Guid QuizId { get; set; }

        // the order the section will be displayed
        public int Index { get; set; }
        public string Description { get; set; }
        public virtual IList<Question> Questions { get; set; }
        public bool IsValid { get; set; }
    }
}
