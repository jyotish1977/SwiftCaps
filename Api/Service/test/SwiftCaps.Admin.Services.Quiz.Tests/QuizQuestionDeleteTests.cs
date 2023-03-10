using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public class QuizQuestionDeleteTests : CRUDTestBase
    {

        [Fact]
        public async Task InvalidPayload_Should_Error()
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            await Should.ThrowAsync<ArgumentException>(async () =>
             await adminQuestionService.DeleteQuestionAsync(default(Guid)));
        }

        [Fact]
        public async Task ValidPayload_Should_DeleteQuestion()
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

            using var context = GetDbContext();
            await Seed(new List<SCModels.Quiz>() { quiz });
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var result = await adminQuestionService.DeleteQuestionAsync(questionId);
            result.ShouldBeTrue();

            var deletedQuestion = await context.Questions.FindAsync(questionId);
            deletedQuestion.ShouldBeNull();

            var updatedQuiz = await context.Quizzes.FindAsync(quizId);
            updatedQuiz.Updated.Value.ShouldNotBe(lastQuizUpdateAt);
        }
    }
}
