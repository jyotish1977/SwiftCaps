using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Data;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xunit;

namespace SwiftCaps.Services.Quiz.Tests
{
    public class QuizServiceTests 
    {
        private DbContextOptions<SwiftCapsContext> _dbContextOptions;
        public QuizServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwiftCapsContext>()
                              .UseInMemoryDatabase("TestDatabase")
                              .Options;
        }

        [Fact]
        public async Task GetAvailableQuizzes_InvalidPayload_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                var quizService = new QuizService(context);
                _ = await Should.ThrowAsync<ArgumentException>(async () => await quizService.GetAvailableUserQuizzes(new UserQuizRequest()));
        }

        [Fact]
        public async Task GetAvailableQuizzes_ValidPayload_Should_ReturnUserQuizzes()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                await SeedAsync(context);
                var quizService = new QuizService(context);
                var response = await quizService.GetAvailableUserQuizzes(new UserQuizRequest{
                        UserId = FakeUserData.Data[0].Id,
                        ClientLocalDateTime = DateTime.Now
                    });
                response.ShouldNotBeNull();
                response.Data.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void AddUserQuiz_EmptyPayload_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                var quizService = new QuizService(context);
                _ = Should.Throw<ArgumentNullException>(() => quizService.AddUserQuiz(null));
        }

        [Fact]
        public void AddUserQuiz_InvalidPayload_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                var quizService = new QuizService(context);
                _ = Should.Throw<ArgumentException>(() => quizService.AddUserQuiz(new UserQuiz()));
        }

        [Fact]
        public async Task AddUserQuiz_ValidPayload_Should_SetUpUserQuiz()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                await SeedAsync(context);
                var quizService = new QuizService(context);
                var quizzes = await quizService.GetAvailableUserQuizzes(new UserQuizRequest
                {
                    UserId = GenericIdentifiers._2ID,
                    ClientLocalDateTime = DateTime.Now
                });
                var userQuiz = await quizService.AddUserQuiz(quizzes.Data.First());
                userQuiz.ShouldNotBeNull();
                userQuiz.Data.UserId.ShouldBe(GenericIdentifiers._2ID);
                userQuiz.Data.Schedule.ShouldNotBeNull();
        }

        [Fact]
        public async Task SaveUserQuiz_EmptyPayload_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                var quizService = new QuizService(context);
                await Should.ThrowAsync<ArgumentNullException>(async () => await quizService.SaveUserQuiz(null));
        }

        [Fact]
        public async Task SaveUserQuiz_InvalidPayload_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                var quizService = new QuizService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await quizService.SaveUserQuiz(new UserQuiz()));
        }

        [Fact]
        public async Task SaveUserQuiz_ValidPayload_Should_Save()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                await SeedAsync(context);
                var quizService = new QuizService(context);
                var quizzes = await quizService.GetAvailableUserQuizzes(new UserQuizRequest { 
                    UserId = GenericIdentifiers._2ID,
                    ClientLocalDateTime = DateTime.Now
                });
                var response = await quizService.SaveUserQuiz(new UserQuiz()
                {
                    ScheduleId = quizzes.Data[0].ScheduleId,
                    UserId = GenericIdentifiers._2ID, 
                    Completed = DateTime.Now,
                    Sequence = 10
                });
        }

        [Fact]
        public async Task SaveUserQuiz_ValidPayload_DuplicateSubmission_Should_Error()
        {
            using var context = new SwiftCapsContext(_dbContextOptions);
                await SeedAsync(context);
                var quizService = new QuizService(context);
                var quizzes = await quizService.GetAvailableUserQuizzes(new UserQuizRequest
                {
                    UserId = GenericIdentifiers._5ID,
                    ClientLocalDateTime = DateTime.Now
                });
                _ = await quizService.SaveUserQuiz(new UserQuiz()
                {
                    ScheduleId = quizzes.Data[0].ScheduleId,
                    UserId = GenericIdentifiers._5ID,
                    Completed = DateTime.Now,
                    Sequence = 10
                });
                await Should.ThrowAsync<InvalidOperationException>(async () => await quizService.SaveUserQuiz(new UserQuiz()
                {
                    ScheduleId = quizzes.Data[0].ScheduleId,
                    UserId = GenericIdentifiers._5ID,
                    Completed = DateTime.Now,
                    Sequence = 10
                }));
        }

        private async Task SeedAsync(SwiftCapsContext context)
        {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                await context.Groups.AddRangeAsync(FakeGroupData.Data);
                await context.Users.AddRangeAsync(FakeUserData.Data);
                await context.Quizzes.AddRangeAsync(FakeQuizData.Data);
                await context.QuizSections.AddRangeAsync(FakeQuizSectionData.Data);
                await context.Questions.AddRangeAsync(FakeQuestionData.Data);
                await context.Schedules.AddRangeAsync(FakeQuizScheduleData.Data);
                await context.ScheduleGroups.AddRangeAsync(FakeQuizScheduleGroupData.Data);
                await context.UserQuizzes.AddRangeAsync(FakeUserQuizData.Data);

                context.SaveChanges();
        }
    }
}
