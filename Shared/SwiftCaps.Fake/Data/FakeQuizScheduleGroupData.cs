using System;
using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizScheduleGroupData
    {
        static FakeQuizScheduleGroupData()
        {
            Init();
        }

        public static List<ScheduleGroup> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<ScheduleGroup>
            {
                new ScheduleGroup
                {
                    Id = GenericIdentifiers._1ID,
                    ScheduleId = GenericIdentifiers._1ID,
                    GroupId = GenericIdentifiers._101ID
                },
                new ScheduleGroup
                {
                    Id = GenericIdentifiers._2ID ,
                    ScheduleId = GenericIdentifiers._1ID,
                    GroupId = GenericIdentifiers._102ID
                },
                new ScheduleGroup
                {
                    Id = GenericIdentifiers._3ID,
                    ScheduleId = GenericIdentifiers._2ID,
                    GroupId = GenericIdentifiers._101ID
                },
                new ScheduleGroup
                {
                    Id = GenericIdentifiers._4ID,
                    ScheduleId = GenericIdentifiers._2ID,
                    GroupId = GenericIdentifiers._102ID
                }
            };
        }
    }
}
