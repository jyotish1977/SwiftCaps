using System.Collections.Generic;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Models.Models
{
    public class Quiz : CoreObject
    {
        public string Name { get; set; }

        public string Description { get; set; }

        // this is a markdown information that is available during the quiz
        public string InfoMarkdown { get; set; }

        // All questions are contained within a Quiz Section
        // even is there is only a single question in a quiz section
        public virtual IList<QuizSection> QuizSections { get; set; }

        public virtual IList<Schedule> Schedules { get; set; }

    }
}
