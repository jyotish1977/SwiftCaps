using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizQuestionCreateTests : CRUDTestBase
    {
        public static IEnumerable<object[]> InvalidPayloadForNullException()
        {
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadForNullException))]
        public async Task NoPayload_Should_Error(SCModels.Question payload)
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            await Should.ThrowAsync<ArgumentNullException>(async () =>
                await adminQuestionService.CreateQuestionAsync(payload));
        }


        public static IEnumerable<object[]> InvalidPayloadForArgumentException()
        {
            yield return new object[] { new SCModels.Question() };
            yield return new object[] { new SCModels.Question() {
                QuizSectionId = default,
                Body = null 
            } };
            yield return new object[] { new SCModels.Question() {
                QuizSectionId = Guid.NewGuid(),
                Body = null
            } }; 
            yield return new object[] { new SCModels.Question() {
                QuizSectionId = default,
                Body = "test question body"
            } };
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadForArgumentException))]
        public async Task InvalidPayload_Should_Error(SCModels.Question payload)
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            await Should.ThrowAsync<ArgumentException>(async () =>
                await adminQuestionService.CreateQuestionAsync(payload));
        }

        [Fact]
        public async Task ValidPayload_Should_CreateQuestion()
        {
            var lastQuizUpdateAt = DateTime.UtcNow.AddDays(-1);
            var quizId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var quiz = new SCModels.Quiz
            {
                Id = quizId,
                Name = "Quiz 1",
                Description = "Quiz 1 description",
                Created = lastQuizUpdateAt,
                Updated = lastQuizUpdateAt,
                QuizSections = new List<SCModels.QuizSection>
                {
                    new SCModels.QuizSection
                    {
                        Id = sectionId,
                        QuizId = quizId,
                        Description = "section 1",
                        Index = 1,
                        Created = lastQuizUpdateAt,
                        Updated = lastQuizUpdateAt,
                        Questions = new List<SCModels.Question>
                        {
                            new SCModels.Question {
                                Id = questionId,
                                QuizSectionId = sectionId,
                                QuizSectionIndex = 1,
                                Header = "header 1",
                                Body = "body",
                                Footer = "footer",
                                Created = lastQuizUpdateAt,
                                Updated = lastQuizUpdateAt
                            }
                        }
                    }
                }
            };

            var payload = new SCModels.Question()
            {
                QuizSectionId = sectionId,
                Body = "test question body"
            };

            using var context = GetDbContext();
            await Seed(new List<SCModels.Quiz>() { quiz });
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var newId = await adminQuestionService.CreateQuestionAsync(payload);
            newId.ShouldNotBeNull();

            var createdQuestion = await context.Questions.FindAsync(newId);
            createdQuestion.ShouldNotBeNull();
            createdQuestion.QuizSectionId.ShouldBe(payload.QuizSectionId);
            createdQuestion.QuizSectionIndex.ShouldBe(2);
            createdQuestion.Body.ShouldBe(payload.Body);
            createdQuestion.Description.ShouldBe(payload.Description);

            var updatedQuiz = await context.Quizzes.FindAsync(quizId);
            updatedQuiz.Updated.Value.ShouldNotBe(lastQuizUpdateAt);
        }


    }
}
