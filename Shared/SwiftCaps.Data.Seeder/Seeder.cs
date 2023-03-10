using Microsoft.EntityFrameworkCore;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Data;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SwiftCaps.Data.Seeder
{
    public static class Seeder
    {
        public static void Seed(IConfiguration config)
        {
            try
            {
                var ob = new DbContextOptionsBuilder<SwiftCapsContext>();
                var conn = config["SqlConnectionString"];
                ob.UseSqlServer(conn);

                using (var context = new SwiftCapsContext(ob.Options))
                {
                    context.Groups.AddRange(FakeGroupData.Data);
                    context.Users.AddRange(FakeUserData.Data);
                    context.SaveChanges();
                    context.Quizzes.AddRange(FakeQuizData.Data);
                    context.SaveChanges();

                    if (!context.QuizSections.Any())
                    {
                        context.QuizSections.AddRange(FakeQuizSectionData.Data);
                        try
                        {
                            context.SaveChanges();
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    if (!context.Questions.Any())
                    {
                        context.Questions.AddRange(FakeQuestionData.Data);
                        context.SaveChanges();
                    }

                    if (!context.Schedules.Any())
                    {
                        context.Schedules.AddRange(FakeQuizScheduleData.Data);
                        context.SaveChanges();
                    }

                    if (!context.ScheduleGroups.Any())
                    {
                        context.ScheduleGroups.AddRange(FakeQuizScheduleGroupData.Data);
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)

            {
                // ignored
            }
        }
    }
}
