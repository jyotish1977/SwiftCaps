using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{

    public class QuizSectionUpdateTests : CRUDTestBase
    {
        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            Guid? payloadId = null;
            SCModels.QuizSection payload = null;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.UpdateSectionAsync(payloadId.GetValueOrDefault(), payload));
        }

        [Fact]
        public async Task InvalidPayload_SectionIdEmpty_Should_Error()
        {
            Guid? payloadId = Guid.Empty;
            SCModels.QuizSection payload = default;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.UpdateSectionAsync(payloadId.GetValueOrDefault(), payload));
        }

        [Fact]
        public async Task InvalidPayload_DefaultPayload_Should_Error()
        {
            Guid? payloadId = Guid.Empty;
            SCModels.QuizSection payload = default;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.UpdateSectionAsync(payloadId.GetValueOrDefault(), payload));
        }

        [Fact]
        public async Task ValidPayload_NonExisitingSection_Should_Error()
        {
            Guid payloadId = Guid.NewGuid();
            SCModels.QuizSection payload = new SCModels.QuizSection
            {
                Description = "Section 1",
            };

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<NotFoundException>(async () => await sectionService.UpdateSectionAsync(payloadId, payload));
        }

        [Fact]
        public async Task ValidPayload_Should_ReturnSection()
        {
            var newQuizId = Guid.NewGuid();
            var newSectionId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new[] {
                       new SCModels.Quiz
                       {
                           Id = newQuizId,
                           Name = " Quiz 1",
                           Description = "Quiz 1",
                           Created = DateTime.UtcNow.AddDays(-1),
                           Updated = DateTime.UtcNow.AddDays(-1),
                           QuizSections = new []
                           {
                               new SCModels.QuizSection
                               {
                                   Id = newSectionId,
                                   Description = "Section 1",
                                   Index = 1,
                                   Created = DateTime.UtcNow.AddDays(-1),
                                   Updated = DateTime.UtcNow.AddDays(-1),
                                   QuizId = newQuizId
                               }
                            }
                       }
                    });
                await context.SaveChangesAsync();

                var payload = new SCModels.QuizSection
                {
                    Description = "Section 1",
                    QuizId = newQuizId
                };
                var updatedSectionId = await sectionService.UpdateSectionAsync(newSectionId, payload);
                updatedSectionId.ShouldNotBeNull();
                updatedSectionId.ShouldBeOfType<Guid>();

                var newSection = await sectionService.GetSectionAsync(newSectionId);
                newSection.Index.ShouldBe(1);
                newSection.Updated.Value.ToUniversalTime().Date.ShouldBe(DateTime.UtcNow.Date);

                var quiz = await context.Quizzes.FindAsync(newQuizId);
                quiz.Updated.ShouldNotBeNull();
                quiz.Updated.Value.ToUniversalTime().Date.ShouldBe(DateTime.UtcNow.Date);
        }
    }
}
