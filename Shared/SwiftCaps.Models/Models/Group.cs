using System.Collections.Generic;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Models.Models
{
    public class Group : CoreObject
    {
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        
    }
}
