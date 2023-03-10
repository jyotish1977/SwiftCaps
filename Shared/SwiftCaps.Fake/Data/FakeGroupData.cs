using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeGroupData
    {
        static FakeGroupData()
        {
            Init();
        }

        public static List<Group> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<Group>
            {
                new Group {Id = GenericIdentifiers._101ID , Name = "Grizzly Bears"},
                new Group {Id = GenericIdentifiers._102ID , Name = "Gold Pandas"},
                new Group {Id = GenericIdentifiers._103ID , Name = "Polar Bears"},
            };
        }
    }
}
