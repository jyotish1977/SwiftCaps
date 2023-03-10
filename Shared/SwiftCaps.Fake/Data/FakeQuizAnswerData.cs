using System.Collections.Generic;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeQuizAnswerData
    {
        static FakeQuizAnswerData()
        {
            Init();
        }

        public static List<QuizAnswer> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<QuizAnswer>
            {
                new QuizAnswer
                {
                    
                }
            };
        }
    }
}
