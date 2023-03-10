using System;

namespace SwiftCaps.Models.Requests
{
    public class AdminReportingRequest
    {
        public Guid GroupId { get; set; }
        public DateTimeOffset ClientLocalDateTime { get; set; }
    }
}
