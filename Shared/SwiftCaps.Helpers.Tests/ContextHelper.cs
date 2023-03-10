using Microsoft.EntityFrameworkCore;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Data;
using SwiftCaps.Helpers.Tests.Extensions;

namespace SwiftCaps.Helpers.Tests
{
    public static class ContextHelper
    {
        public static SwiftCapsContext GenerateSwiftcapsContext()
        {
            FakeGroupData.Init();
            FakeUserData.Init();
            FakeQuizData.Init();
            FakeQuestionData.Init();
            FakeQuizSectionData.Init();
            FakeQuizAnswerData.Init();
            FakeQuizScheduleData.Init();
            FakeUserQuizData.Init();
            var context = new SwiftCapsContext(new DbContextOptions<SwiftCapsContext>())
            {
                Groups = FakeGroupData.Data.ToDbSet(),
                Users = FakeUserData.Data.ToDbSet(),
                Quizzes = FakeQuizData.Data.ToDbSet(),
                QuizSections = FakeQuizSectionData.Data.ToDbSetModel(),
                Questions = FakeQuestionData.Data.ToDbSetModel(),
                Schedules = FakeQuizScheduleData.Data.ToDbSet(),
                UserQuizzes = FakeUserQuizData.Data.ToDbSet(),
            };
            return context;
        }
    }
}
