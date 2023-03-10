using System;

namespace SwiftCaps.Models.Requests
{
    public class UserQuizRequest
    {
        public Guid UserId { get; set; } 
        public DateTimeOffset ClientLocalDateTime { get; set; }
    }
}
