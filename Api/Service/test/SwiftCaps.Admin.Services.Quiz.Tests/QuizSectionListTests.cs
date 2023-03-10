using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using SwiftCaps.Fake.Data;
using Xunit;
using SCModels=SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizSectionListTests : CRUDTestBase
    {
        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            Guid? payload = null;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.GetSectionsAsync(payload.GetValueOrDefault()));
        }

        [Fact]
        public async Task InvalidPayload_Should_Error()
        {
            Guid? emptyGuidPayload = Guid.Empty;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.GetSectionsAsync(emptyGuidPayload.GetValueOrDefault()));
        }
      
        [Fact]
        public async Task ValidPayload_NonExisitingQuiz_Should_Error()
        {
            Guid? quizIdPayload = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<NotFoundException>(async () => await sectionService.GetSectionsAsync(quizIdPayload.GetValueOrDefault()));
        }

        [Fact]
        public async Task ValidPayload_QuizWithNoSections_Should_ReturnEmptyList()
        {
            var newQuizId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new [] 
                { 
                    new SCModels.Quiz
                    { 
                        Id=newQuizId,Name="Quiz 1",Description="Quiz 1" 
                    } 
                });
                await context.SaveChangesAsync();
                
                var payload = newQuizId;
                var sections = await sectionService.GetSectionsAsync(payload);
                sections.ShouldNotBeNull();
                sections.Count.ShouldBe(0);
        }

        [Fact]
        public async Task ValidPayload_QuizWithSections_Should_ReturnSections()        
        {
            var newQuizId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new [] 
                    { 
                        new SCModels.Quiz
                        { 
                            Id=newQuizId,Name="Quiz 1",Description="Quiz 1" 
                        } 
                    });
                await Seed(new [] {
                    new SCModels.QuizSection{ Description = "Section 1", Index=0, QuizId=newQuizId },
                    new SCModels.QuizSection{ Description = "Section 2", Index=1, QuizId=newQuizId },
                    new SCModels.QuizSection{ Description = "Section 3", Index=2, QuizId=newQuizId },
                });
                await context.SaveChangesAsync();
                
                var payload = newQuizId;
                var expectedSectionCount = 3;
                var sections = await sectionService.GetSectionsAsync(payload);
                sections.ShouldNotBeNull();
                sections.Count.ShouldBe(expectedSectionCount);
        }
    }
}
