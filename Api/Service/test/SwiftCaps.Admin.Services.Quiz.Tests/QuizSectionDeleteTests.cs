using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{

    public class QuizSectionDeleteTests : CRUDTestBase
    {
        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            Guid? payloadId = null;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await sectionService.DeleteSectionAsync(payloadId.GetValueOrDefault()));
        }

        [Fact]
        public async Task InvalidPayload_Should_Error()
        {
            Guid? payloadId = Guid.Empty;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await sectionService.DeleteSectionAsync(payloadId.GetValueOrDefault()));
        }

        
        [Fact]
        public async Task ValidPayload_NonExisitingSection_Should_Error()
        {
            Guid payloadId = Guid.NewGuid();
            
            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<NotFoundException>(async () => 
                    await sectionService.DeleteSectionAsync(payloadId));
        }

        [Fact]
        public async Task ValidPayload_Should_DeleteSection()
        {
            var newQuizId = Guid.NewGuid();
            var newSection1Id = Guid.NewGuid();
            var newSection2Id = Guid.NewGuid();
            var newSection3Id = Guid.NewGuid();

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
                           QuizSections = new List<SCModels.QuizSection>
                           {
                               new SCModels.QuizSection
                               {
                                   Id = newSection1Id,
                                   Description = "Section 1",
                                   Index = 1,
                                   Created = DateTime.UtcNow.AddDays(-1),
                                   Updated = DateTime.UtcNow.AddDays(-1),
                                   QuizId = newQuizId
                               },
                               new SCModels.QuizSection
                               {
                                   Id = newSection2Id,
                                   Description = "Section 2",
                                   Index = 2,
                                   Created = DateTime.UtcNow.AddDays(-1),
                                   Updated = DateTime.UtcNow.AddDays(-1),
                                   QuizId = newQuizId
                               },
                               new SCModels.QuizSection
                               {
                                   Id = newSection3Id,
                                   Description = "Section 3",
                                   Index = 3,
                                   Created = DateTime.UtcNow.AddDays(-1),
                                   Updated = DateTime.UtcNow.AddDays(-1),
                                   QuizId = newQuizId
                               }
                            }
                       }
                    });
                await context.SaveChangesAsync();

                var payloadId = newSection2Id;

                var deleteSuccess = await sectionService.DeleteSectionAsync(payloadId);
                deleteSuccess.ShouldBeTrue();

                var quiz = await context.Quizzes
                                        .Include(quiz => quiz.QuizSections)
                                        .SingleOrDefaultAsync(quiz => quiz.Id == newQuizId);
                quiz.Updated.ShouldNotBeNull();
                quiz.Updated.Value.ToUniversalTime().Date.ShouldBe(DateTime.UtcNow.Date);
                quiz.QuizSections.Count.ShouldBe(2);
                quiz.QuizSections[1].Index.ShouldBe(2);
        }
    }
}
