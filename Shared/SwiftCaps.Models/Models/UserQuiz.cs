using System;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Models.Models
{
    public class UserQuiz : CoreObject
    {
        public Guid UserId { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime? Completed { get; set; }
        public int Sequence { get; set; }
        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }
    }
}
