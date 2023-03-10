using System;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Models.Models
{
    public class ScheduleGroup : CoreObject
    {
        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

    }
}
