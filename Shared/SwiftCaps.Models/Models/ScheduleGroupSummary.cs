using System;

namespace SwiftCaps.Models.Models
{
    public class ScheduleGroupSummary
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public int UserCount { get; set; }
    }
}
