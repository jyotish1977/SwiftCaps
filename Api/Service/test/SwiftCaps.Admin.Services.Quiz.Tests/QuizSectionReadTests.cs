using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizSectionReadTests : CRUDTestBase
    {

        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            Guid? payload = null;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.GetSectionAsync(payload.GetValueOrDefault()));
        }

        [Fact]
        public async Task InvalidPayload_Should_Error()
        {
            Guid? emptyGuidPayload = Guid.Empty;

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<ArgumentException>(async () => await sectionService.GetSectionAsync(emptyGuidPayload.GetValueOrDefault()));
        }
      
        [Fact]
        public async Task ValidPayload_NonExisitingSection_Should_Error()
        {
            Guid? sectionIdPayload = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Should.ThrowAsync<NotFoundException>(async () => await sectionService.GetSectionAsync(sectionIdPayload.GetValueOrDefault()));
        }

        [Fact]
        public async Task ValidPayload_SectionWithoutQuestion_Should_ReturnSection()
        {
            var newQuizId = Guid.NewGuid();
            var newSectionId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new [] { 
                   new SCModels.Quiz { Id = newQuizId, Name = " Quiz 1", Description = "Quiz 1" } 
                });
                await Seed(new [] {
                    new SCModels.QuizSection
                    {
                        Id = newSectionId, Description = "Section 1", Index = 0, QuizId = newQuizId
                    }                        
                });
                await context.SaveChangesAsync();
                
                var payload = newSectionId;
                var section = await sectionService.GetSectionAsync(payload);
                section.ShouldNotBeNull();
                section.Id.ShouldBe(newSectionId);
                section.QuizId.ShouldBe(newQuizId);
                section.Description.ShouldBe("Section 1");
                section.Questions.Count.ShouldBe(0);
        }

       [Fact]
        public async Task ValidPayload_SectionWithQuestion_Should_ReturnSection()
        {
            var newQuizId = Guid.NewGuid();
            var newSectionId = Guid.NewGuid();

            using var context = GetDbContext();
                var sectionService = new AdminQuizSectionService(context);
                await Seed(new [] { 
                   new SCModels.Quiz { Id = newQuizId, Name = " Quiz 1", Description = "Quiz 1" } 
                });
                await Seed(new [] {
                    new SCModels.QuizSection
                    {
                        Id = newSectionId, 
                        Description = "Section 1", 
                        Index = 0, 
                        QuizId = newQuizId,
                        Questions = new [] 
                        { 
                            new SCModels.Question { Body="Question 2 {foo}", QuizSectionIndex=1,QuizSectionId=newSectionId },
                            new SCModels.Question { Body="Question 1 {foo}", QuizSectionIndex=0,QuizSectionId=newSectionId }
                        }
                    }                        
                });
                await context.SaveChangesAsync();
                
                var payload = newSectionId;
                var section = await sectionService.GetSectionAsync(payload);
                section.ShouldNotBeNull();
                section.Id.ShouldBe(newSectionId);
                section.QuizId.ShouldBe(newQuizId);
                section.Description.ShouldBe("Section 1");
                section.Questions.Count.ShouldBe(2);
                section.Questions[0].QuizSectionIndex.ShouldBe(0);
        }
    }
}
