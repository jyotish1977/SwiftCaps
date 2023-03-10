using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using SwiftCaps.Fake.Data;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizDeleteTests : CRUDTestBase
    {
        [Fact]
        public async Task NoPayload_Should_Error()
        {
            Guid? quizIdToDelete = null;

            using var context = GetDbContext();
            var adminQuizService = new AdminQuizService(context);
            await Should.ThrowAsync<ArgumentException>(async () =>
                await adminQuizService.DeleteQuizAsync(quizIdToDelete.GetValueOrDefault()));
        }

        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            var quizIdToDelete = Guid.Empty;

            using var context = GetDbContext();
            var adminQuizService = new AdminQuizService(context);
            await Should.ThrowAsync<ArgumentException>(async () =>
                await adminQuizService.DeleteQuizAsync(quizIdToDelete));
        }

        [Fact]
        public async Task ValidPayload_NonExistingQuiz_Should_Error()
        {
            var quizIdToDelete = Guid.NewGuid();

            using var context = GetDbContext();
            var adminQuizService = new AdminQuizService(context);
            await Should.ThrowAsync<NotFoundException>(async () =>
                await adminQuizService.DeleteQuizAsync(quizIdToDelete));
        }

        [Fact]
        public async Task ValidPayload_Should_UpdateQuiz()
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

                var result = await adminQuizService.DeleteQuizAsync(newId.Value);
                result.ShouldBeTrue();

                var updatedQuiz = await context.Quizzes.FindAsync(newId.Value);
                updatedQuiz.ShouldBeNull();
        }

    }
}
