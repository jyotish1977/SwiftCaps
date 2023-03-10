using System.Threading.Tasks;
using Shouldly;
using SwiftCaps.Fake.Data;
using Xunit;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizListTests : CRUDTestBase
    {
        
        [Fact]
        public async Task GetQuizzes_EmptyDb_Should_ReturnEmptyList()
        {
            using var context = GetDbContext();
                var adminQuizService = new AdminQuizService(context);
                var result = await adminQuizService.GetQuizzesAsync();
                result.ShouldNotBeNull();
                result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetQuizzes_NotEmptyDb_Should_ReturnQuizList()
        {
            using var context = GetDbContext();
                await Seed(FakeGroupData.Data);
                await Seed(FakeQuizData.Data);
                await Seed(FakeQuizSectionData.Data);
                await Seed(FakeQuestionData.Data);
                await Seed(FakeQuizScheduleData.Data);
                await context.SaveChangesAsync();
                var adminQuizService = new AdminQuizService(context);
                var result = await adminQuizService.GetQuizzesAsync();
                result.ShouldNotBeNull();
                result.Count.ShouldBe(FakeQuizData.Data.Count);
        }

        
    }
}
