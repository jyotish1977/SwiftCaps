using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScenarioTests;
using Shouldly;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class ProgressReportTests : CRUDTestBase
    {
        [Fact]
        public async Task When_NoSchedules_Should_GenerateReport()
        {
            using var context = GetDbContext();
                await SeedGroups(context);
                var sut = new AdminReportingService(context);
                var report = await sut.GetGroupProgressReport(new AdminReportingRequest{
                    ClientLocalDateTime = DateTime.Now
                });
                report.Count.ShouldBe(2);
        }

        [Fact]
        public async Task When_ScheduleAssigned_UserNotCompletedQuizzes_Should_GenerateReport()
        {
            using var context = GetDbContext();
                await SeedGroups(context);
                await SeedUsers(context);
                await SeedQuizzes(context);
                await SeedSchedules(context);
                await SeedScheduleGroups(context);

                var sut = new AdminReportingService(context);
                var report = await sut.GetGroupProgressReport(new AdminReportingRequest{
                    ClientLocalDateTime = DateTime.Now
                });
                report.Count.ShouldBe(2);
                report[0].AvergageTime.ShouldBeNullOrEmpty();
                report[0].DonePercentage.ShouldBeNull();
                report[0].GroupName.ShouldNotBeNullOrEmpty();
                report[0].QuizName.ShouldNotBeNullOrEmpty();
                report[0].Recurrence.ShouldNotBeNull();
                report[0].UserCount.ShouldBe(5);
        }

        [Fact]
        public async Task When_ScheduleAssigned_NoUsersInGroup_Should_GenerateReport()
        {
            using var context = GetDbContext();
                await SeedGroups(context);
                await SeedQuizzes(context);
                await SeedSchedules(context);
                await SeedScheduleGroups(context);

                var sut = new AdminReportingService(context);
                var report = await sut.GetGroupProgressReport(new AdminReportingRequest{
                    ClientLocalDateTime = DateTime.Now
                });
                report.Count.ShouldBe(2);
                report[0].AvergageTime.ShouldBeNullOrEmpty();
                report[0].DonePercentage.ShouldBeNull();
                report[0].GroupName.ShouldNotBeNullOrEmpty();
                report[0].QuizName.ShouldNotBeNullOrEmpty();
                report[0].Recurrence.ShouldNotBeNull();
                report[0].UserCount.ShouldBe(0);
        }

        [Fact]
        public async Task When_ScheduleAssigned_GroupAssigned_UserCompletedQuizzes_Should_GenerateReport()
        {
            using var context = GetDbContext();
                await SeedTestData(context);
                var sut = new AdminReportingService(context);
                var report = await sut.GetGroupProgressReport(new AdminReportingRequest{
                    ClientLocalDateTime = DateTime.Now
                });
                report.Count.ShouldBe(2);
                report[0].UserCount.ShouldBe(5);
                report[0].DonePercentage.ShouldBe(100);
                report[0].AvergageTime.ShouldBe("9m 0s");

                report[1].UserCount.ShouldBe(5);
                report[1].DonePercentage.ShouldBe(80);
                report[1].AvergageTime.ShouldBe("11m 15s");
        }

        


        private async Task SeedTestData(Data.Context.SwiftCapsContext context)
        {
            await SeedGroups(context);
            await SeedUsers(context);
            await SeedQuizzes(context);
            await SeedSchedules(context);
            await SeedScheduleGroups(context);
            await SeedGroup1UserQuiz(context);
            await SeedGroup2UserQuiz(context);
        }

        private async Task SeedGroup1UserQuiz(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<UserQuiz>
            {
                new UserQuiz
                {
                    Id = GenericIdentifiers._91ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    UserId = GenericIdentifiers._11ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(5),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._92ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    UserId = GenericIdentifiers._12ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(10),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._93ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    UserId = GenericIdentifiers._13ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(15),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._94ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    UserId = GenericIdentifiers._14ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(10),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._95ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    UserId = GenericIdentifiers._15ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(5),
                }
            });
            await context.SaveChangesAsync();
        }

        private async Task SeedGroup2UserQuiz(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<UserQuiz>
            {
                new UserQuiz
                {
                    Id = GenericIdentifiers._96ID,
                    ScheduleId = GenericIdentifiers._52ID,
                    UserId = GenericIdentifiers._16ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(10),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._97ID,
                    ScheduleId = GenericIdentifiers._52ID,
                    UserId = GenericIdentifiers._17ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(20),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._98ID,
                    ScheduleId = GenericIdentifiers._52ID,
                    UserId = GenericIdentifiers._18ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(5),
                },
                new UserQuiz
                {
                    Id = GenericIdentifiers._99ID,
                    ScheduleId = GenericIdentifiers._52ID,
                    UserId = GenericIdentifiers._19ID,
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow.AddMinutes(10),
                }
            });
            await context.SaveChangesAsync();
        }

        private async Task SeedScheduleGroups(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<ScheduleGroup>{
                new ScheduleGroup {
                    Id = GenericIdentifiers._71ID,
                    ScheduleId = GenericIdentifiers._51ID,
                    GroupId = GenericIdentifiers._1ID,
                    Created = DateTime.UtcNow
                },
                new ScheduleGroup {
                    Id = GenericIdentifiers._72ID,
                    ScheduleId = GenericIdentifiers._52ID,
                    GroupId = GenericIdentifiers._2ID,
                    Created = DateTime.UtcNow
                }
            });
            await context.SaveChangesAsync();
        }

        private async Task SeedSchedules(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<Schedule>
            {
                new Schedule { Id = GenericIdentifiers._51ID,
                               QuizId=GenericIdentifiers._31ID,
                               Recurrence= Recurrence.Monthly,
                               StartTime = DateTime.UtcNow,
                               EndTime = DateTime.UtcNow.AddMonths(3),
                               Created = DateTime.UtcNow,
                             },
                new Schedule { Id = GenericIdentifiers._52ID,
                               QuizId=GenericIdentifiers._32ID,
                               Recurrence= Recurrence.Weekly,
                               StartTime = DateTime.UtcNow,
                               EndTime = DateTime.UtcNow.AddMonths(3),
                               Created = DateTime.UtcNow,
                             },

            });
            await context.SaveChangesAsync();
        }

        private async Task SeedQuizzes(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<SCModels.Quiz>{
                new SCModels.Quiz{Id=GenericIdentifiers._31ID,Created=DateTime.UtcNow, Name="Quiz 1"},
                new SCModels.Quiz{Id=GenericIdentifiers._32ID,Created=DateTime.UtcNow, Name="Quiz 2"},
            });
            await context.SaveChangesAsync();
        }

        private async Task SeedUsers(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<User>{
                    new User{ Id=GenericIdentifiers._11ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._1ID},
                    new User{ Id=GenericIdentifiers._12ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._1ID},
                    new User{ Id=GenericIdentifiers._13ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._1ID},
                    new User{ Id=GenericIdentifiers._14ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._1ID},
                    new User{ Id=GenericIdentifiers._15ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._1ID},
                    new User{ Id=GenericIdentifiers._16ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._2ID},
                    new User{ Id=GenericIdentifiers._17ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._2ID},
                    new User{ Id=GenericIdentifiers._18ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._2ID},
                    new User{ Id=GenericIdentifiers._19ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._2ID},
                    new User{ Id=GenericIdentifiers._20ID, Created=DateTime.UtcNow, GroupId = GenericIdentifiers._2ID},
                });
            await context.SaveChangesAsync();
        }

        private async Task SeedGroups(Data.Context.SwiftCapsContext context)
        {
            await Seed(new List<Group>{
                    new Group{ Id=GenericIdentifiers._1ID, Name = "Group1"},
                    new Group{ Id=GenericIdentifiers._2ID, Name = "Group2"}
                });
            await context.SaveChangesAsync();
        }
    }
}
