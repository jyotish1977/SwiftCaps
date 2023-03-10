using System;
using System.Threading.Tasks;
using Shouldly;
using SCModels = SwiftCaps.Models.Models;
using Xunit;
using SwiftCaps.Fake.Data;
using Ardalis.GuardClauses;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizUpdateTests : CRUDTestBase
    {
        [Fact]
        public async Task NoPayload_Should_Error()
        {
            Guid? quizIdToUpdate = null;
            SCModels.Quiz payload = null;

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await adminQuizService.UpdateQuizAsync(quizIdToUpdate.GetValueOrDefault(), payload));
        }

        [Fact]
        public async Task EmptyPayload_Should_Error()
        {
            var quizIdToUpdate = Guid.Empty;
            var payload = new SCModels.Quiz();

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await adminQuizService.UpdateQuizAsync(quizIdToUpdate,payload));
        }

        [Theory]
        [InlineData("","description")]
        [InlineData("name","")]
        public async Task InvalidPayload_Should_Error(string name, string description)
        {
            var quizIdToUpdate = Guid.NewGuid();
            var payload = new SCModels.Quiz(){
                                Name = name, 
                                Description = description,
                                CreatedBy = FakeUserData.Data[0].Id
                          };

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<ArgumentException>(async () => 
                    await adminQuizService.UpdateQuizAsync(quizIdToUpdate,payload));
        }

        [Fact]
        public async Task ValidPayload_NonExistingQuiz_Should_Error()
        {
            var quizIdToUpdate = Guid.NewGuid();
            var payload = new SCModels.Quiz(){
                                Name = "Quiz 123", 
                                Description = "Quiz description",
                                InfoMarkdown = "Quiz info",
                                CreatedBy = FakeUserData.Data[0].Id
                          };

            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                await Should.ThrowAsync<NotFoundException>(async () => 
                    await adminQuizService.UpdateQuizAsync(quizIdToUpdate, payload));
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
                
                payload.Name += " Edited"; 
                payload.Description += " Edited"; 
                payload.InfoMarkdown += " Edited"; 
                
                var updatedId = await adminQuizService.UpdateQuizAsync(newId.Value, payload);

                var updatedQuiz = await context.Quizzes.FindAsync(updatedId);
                updatedQuiz.ShouldNotBeNull();
                updatedQuiz.Name.ShouldBe("Quiz 123 Edited");
                updatedQuiz.Description.ShouldBe("Quiz description Edited");
                updatedQuiz.InfoMarkdown.ShouldBe("Quiz info Edited");
        }


    }
}
