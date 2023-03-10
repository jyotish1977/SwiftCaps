using System;
using Xamariners.Core.Model;

namespace SwiftCaps.Models.Models
{
    public class User : MemberBase
    {
        public Guid GroupId { get; set; }
        public string Email { get; set; }
    }
}
