using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using SwiftCaps.Fake.Data;
using Xunit;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizQuestionReadTests : CRUDTestBase
    {
        [Fact]
        public async Task InvalidQuestionIdentifier_Should_ReturnError()
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            var exception = await Should.ThrowAsync<ArgumentException>(async () =>
                await adminQuestionService.GetQuestionAsync(Guid.Empty));
        }


        [Fact]
        public async Task NonExistingQuestionIdentifier_Should_ReturnNotFound()
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            var exception = await Should.ThrowAsync<NotFoundException>(async ()
                => await adminQuestionService.GetQuestionAsync(Guid.NewGuid()));
        }


        [Fact]
        public async Task ValidQuestionIdentifier_Should_ReturnQuiz()
        {
            using var context = GetDbContext();
            await Seed(FakeQuizData.Data);
            await Seed(FakeQuizSectionData.Data);
            await Seed(FakeQuestionData.Data);
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var question = await adminQuestionService.GetQuestionAsync(FakeQuestionData.Data[0].Id);
            question.ShouldNotBeNull();
            question.Id.ShouldBe(FakeQuestionData.Data[0].Id);
            question.Body.ShouldBe(FakeQuestionData.Data[0].Body);
        }


    }
}
