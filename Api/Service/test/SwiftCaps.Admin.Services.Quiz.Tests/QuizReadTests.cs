using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Shouldly;
using SwiftCaps.Fake.Data;
using Xunit;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizReadTests : CRUDTestBase
    {
        [Fact]
        public async Task GetQuiz_InvalidQuizIdentifier_Should_ReturnError()
        {
            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                var exception = await Should.ThrowAsync<ArgumentException>(async () => 
                    await adminQuizService.GetQuizAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetQuiz_NonExistingQuizIdentifier_Should_ReturnNotFound()
        {
            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                var exception = await Should.ThrowAsync<NotFoundException>(async () 
                    => await adminQuizService.GetQuizAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetQuiz_ValidQuizIdentifier_Should_ReturnQuiz()
        {
            using var context = GetDbContext();
                await Seed(FakeGroupData.Data);
                await Seed(FakeQuizData.Data);
                await Seed(FakeQuizSectionData.Data);
                await Seed(FakeQuestionData.Data);
                await Seed(FakeQuizScheduleData.Data);
                await context.SaveChangesAsync();

                var adminQuizService = new AdminQuizService(context);
                var quiz = await adminQuizService.GetQuizAsync(FakeQuizData.Data[0].Id);
                quiz.ShouldNotBeNull();
                quiz.Id.ShouldBe(FakeQuizData.Data[0].Id);
                quiz.QuizSections.Count.ShouldBe(FakeQuizSectionData.Data.Where(x => x.QuizId == FakeQuizData.Data[0].Id).Count());
        }
    }
}
