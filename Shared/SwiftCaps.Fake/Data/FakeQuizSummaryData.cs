using System;
using System.Collections.Generic;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizSummaryData
    {
        static FakeQuizSummaryData()
        {
            Init();
        }

        public static IList<QuizSummary> Data { get; set; }

        public static void Init()
        {
            var listQuizSummaries = new List<QuizSummary>();
            var randomObj = new Random();
            foreach(var quiz in FakeQuizData.Data)
            {
                listQuizSummaries.Add(new QuizSummary
                {
                    Id = Guid.NewGuid(),
                    Groups = randomObj.Next(0, 20),
                    Questions = randomObj.Next(0, 30),
                    Sections = randomObj.Next(0, 15),
                    Title = quiz.Name
                });
            }

            Data = null;
            Data = listQuizSummaries;
        }
    }
}
