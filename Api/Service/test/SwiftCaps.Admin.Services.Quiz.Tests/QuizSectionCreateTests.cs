using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{

    public class QuizSectionCreateTests : CRUDTestBase
    {
        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            SCModels.QuizSection payload = null;

            using var context = GetDbContext();
            var sectionService = new AdminQuizSectionService(context);
            await Should.ThrowAsync<ArgumentException>(async () => await sectionService.CreateSectionAsync(payload));
        }

        [Fact]
        public async Task InvalidPayload_Should_Error()
        {
            SCModels.QuizSection payload = default;

            using var context = GetDbContext();
            var sectionService = new AdminQuizSectionService(context);
            await Should.ThrowAsync<ArgumentException>(async () => await sectionService.CreateSectionAsync(payload));
        }

        [Fact]
        public async Task ValidPayload_NonExisitingQuiz_Should_Error()
        {
            var newSectionId = Guid.NewGuid();
            SCModels.QuizSection payload = new SCModels.QuizSection
            {
                Id = newSectionId,
                Index = 0,
                Description = "Section 1",
                QuizId = Guid.NewGuid()
            };

            using var context = GetDbContext();
            var sectionService = new AdminQuizSectionService(context);
            await Should.ThrowAsync<NotFoundException>(async () => await sectionService.CreateSectionAsync(payload));
        }

        [Fact]
        public async Task ValidPayload_Should_ReturnSection()
        {
            var newQuizId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new[] {
                       new SCModels.Quiz
                       {
                           Id = newQuizId,
                           Name = " Quiz 1",
                           Description = "Quiz 1",
                           Created = DateTime.UtcNow.AddDays(-1),
                           Updated = DateTime.UtcNow.AddDays(-1)
                       }
                    });
                await context.SaveChangesAsync();

                var payload = new SCModels.QuizSection
                {
                    Description = "Section 1",
                    QuizId = newQuizId
                };
                var newSectionId = await sectionService.CreateSectionAsync(payload);
                newSectionId.ShouldNotBeNull();
                newSectionId.ShouldBeOfType<Guid>();

                var newSection = await sectionService.GetSectionAsync(newSectionId.Value);
                newSection.Index.ShouldBe(1);

                var quiz = await context.Quizzes.FindAsync(newQuizId);
                quiz.Updated.ShouldNotBeNull();
                quiz.Updated.Value.ToUniversalTime().Date.ShouldBe(DateTime.UtcNow.Date);
        }
    }
}
