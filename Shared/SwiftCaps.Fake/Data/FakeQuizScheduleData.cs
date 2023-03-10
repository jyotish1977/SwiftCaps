using System;
using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizScheduleData
    {
        static FakeQuizScheduleData()
        {
            Init();
        }

        public static List<Schedule> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<Schedule>
            {
                new Schedule
                {
                    Id = GenericIdentifiers._1ID ,
                    QuizId = GenericIdentifiers._1ID,
                    Recurrence = Recurrence.Weekly,
                    StartTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 14,0,0).ToUniversalTime(),
                    EndTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 23,59,59).AddDays(7).ToUniversalTime()
                },
                new Schedule
                {
                    Id = GenericIdentifiers._2ID ,
                    QuizId = GenericIdentifiers._2ID,
                    Recurrence = Recurrence.Monthly,
                    StartTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 14,0,0).ToUniversalTime(),
                    EndTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 23,59,59).AddMonths(1).ToUniversalTime()
                }
            };
        }
    }
}
