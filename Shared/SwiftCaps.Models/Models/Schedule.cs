using System;
using System.Collections.Generic;
using SwiftCaps.Models.Enums;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Models.Models
{
    public class Schedule : CoreObject
    {
        public Guid QuizId { get; set; }
        public Recurrence Recurrence { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public virtual Quiz Quiz { get; set; }
        public ICollection<ScheduleGroup> ScheduleGroups { get; set; }
        public ICollection<UserQuiz> UserQuizzes { get; set; }
    }
}
