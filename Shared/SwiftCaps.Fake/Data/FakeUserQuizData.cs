using System;
using System.Collections.Generic;
using System.Linq;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;
using Xamariners.Core.Common.Helpers;

namespace SwiftCaps.Fake.Data
{
    public class FakeUserQuizData
    {
        static FakeUserQuizData()
        {
            Init();
        }

        public static List<UserQuiz> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<UserQuiz>
            {
                new UserQuiz
                {
                    Expiry = DateTime.Now.AddDays(-7).LastDayOfWeek(true),
                    Completed = DateTime.Now.AddDays(-14),
                    Created = DateTime.Now.AddDays(-15),
                    UserId = GenericIdentifiers._1ID,
                    Id = Guid.NewGuid(),
                    ScheduleId = GenericIdentifiers._1ID,
                    Schedule = FakeQuizScheduleData.Data.FirstOrDefault(d=> d.Id == GenericIdentifiers._1ID)
                },
                new UserQuiz
                {
                    Expiry = DateTime.Now.AddMonths(-2).LastDayOfMonth(true),
                    Completed = DateTime.Now.AddMonths(-1),
                    Created = DateTime.Now.AddMonths(-2),
                    UserId = GenericIdentifiers._1ID,
                    Id = Guid.NewGuid(),
                    ScheduleId = GenericIdentifiers._2ID,
                    Schedule = FakeQuizScheduleData.Data.FirstOrDefault(d=> d.Id == GenericIdentifiers._2ID)
                }
            };

        }
    }
}
