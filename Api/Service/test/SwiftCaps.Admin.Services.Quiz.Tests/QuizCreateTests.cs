using System;
using System.Threading.Tasks;
using Shouldly;
using SCModels = SwiftCaps.Models.Models;
using Xunit;
using SwiftCaps.Fake.Data;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizCreateTests : CRUDTestBase
    {
        [Fact]
        public async Task NoPayload_Should_Error()
        {
            SCModels.Quiz payload = null;

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentNullException>(async () => 
                    await adminQuizService.CreateQuizAsync(payload));
        }

        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            var payload = new SCModels.Quiz();

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentNullException>(async () => 
                    await adminQuizService.CreateQuizAsync(payload));
        }

        [Theory]
        [InlineData("","description")]
        [InlineData("name","")]
        public async Task InvalidPayload_Should_Error(string name, string description)
        {
            var payload = new SCModels.Quiz(){
                                Name = name, 
                                Description = description
                          };

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await adminQuizService.CreateQuizAsync(payload));
        }

        [Fact]
        public async Task ValidPayload_Should_CreateQuiz()
        {
            var payload = new SCModels.Quiz()
            {
                Name = "Quiz 123",
                Description = "Quiz description",
                InfoMarkdown = "Quiz info",
                CreatedBy = FakeUserData.Data[0].Id
            };

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                var newId = await adminQuizService.CreateQuizAsync(payload);
                newId.ShouldNotBeNull();
                var createdQuiz = await context.Quizzes.FindAsync(newId);
                createdQuiz.ShouldNotBeNull();
                createdQuiz.Name.ShouldBe("Quiz 123");
                createdQuiz.Description.ShouldBe("Quiz description");
                createdQuiz.InfoMarkdown.ShouldBe("Quiz info");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ValidPayload_InfoMarkDownIsNullOrEmpty_Should_CreateQuiz(string infoMarkdown)
        {
            var payload = new SCModels.Quiz()
            {
                Name = "Quiz 123",
                Description = "Quiz description",
                InfoMarkdown = infoMarkdown,
                CreatedBy = FakeUserData.Data[0].Id
            };

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                var newId = await adminQuizService.CreateQuizAsync(payload);
                newId.ShouldNotBeNull();
                var createdQuiz = await context.Quizzes.FindAsync(newId);
                createdQuiz.ShouldNotBeNull();
                createdQuiz.Name.ShouldBe("Quiz 123");
                createdQuiz.Description.ShouldBe("Quiz description");
                createdQuiz.InfoMarkdown.ShouldBe(infoMarkdown);
        }


    }
}
