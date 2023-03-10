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
    public class QuizQuestionUpdateTests : CRUDTestBase
    {

        public static IEnumerable<object[]> InvalidPayloadForNullException()
        {
            yield return new object[] { default(Guid), null };
            yield return new object[] { Guid.NewGuid(), null };
            yield return new object[] { Guid.NewGuid(), new SCModels.Question() };
            yield return new object[] { Guid.NewGuid(), new SCModels.Question() {
                QuizSectionId = default,
                Body = null
            } };
            yield return new object[] { Guid.NewGuid(), new SCModels.Question() {
                QuizSectionId = Guid.NewGuid(),
                Body = null
            } };
            yield return new object[] { Guid.NewGuid(), new SCModels.Question() {
                QuizSectionId = default,
                Body = "test question body"
            } };
        }

        [Theory]
        [MemberData(nameof(InvalidPayloadForNullException))]
        public async Task NoOrInvalidPayload_Should_Error(Guid questionIdToUpdate, SCModels.Question payload)
        {
            using var context = GetDbContext();
            var adminQuestionService = new AdminQuizQuestionService(context);
            await Should.ThrowAsync<ArgumentException>(async () =>
                await adminQuestionService.UpdateQuestionAsync(questionIdToUpdate, payload));
        }


        [Fact]
        public async Task ValidPayload_NonExistingQuestion_Should_Error()
        {
            var payload = new SCModels.Question()
            {
                QuizSectionId = Guid.NewGuid(),
                QuizSectionIndex = 1,
                Body = "test question body"
            };

            using var context = GetDbContext();
                var adminQuestionService = new AdminQuizQuestionService(context);
                await Should.ThrowAsync<NotFoundException>(async () =>
                    await adminQuestionService.UpdateQuestionAsync(Guid.NewGuid(), payload));
        }

        [Fact]
        public async Task ValidPayload_Should_UpdateQuestion()
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
                QuizSectionIndex = 1,
                Body = "test question body"
            };

            using var context = GetDbContext();
            await Seed(new List<SCModels.Quiz>() { quiz });
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var updatedId = await adminQuestionService.UpdateQuestionAsync(questionId, payload);

            updatedId.ShouldBe(questionId);
            var updatedQuestion = await context.Questions.FindAsync(updatedId);
            updatedQuestion.ShouldNotBeNull();
            updatedQuestion.Body.ShouldBe("test question body");
            updatedQuestion.Updated.Value.ShouldNotBe(updatedQuestion.Created);

            var updatedQuiz = await context.Quizzes.FindAsync(quizId);
            updatedQuiz.Updated.Value.ShouldNotBe(lastQuizUpdateAt);
        }

        [Fact]
        public async Task ValidPayload_SectionSwapped_Should_UpdateQuestion()
        {
            var lastQuizUpdateAt = DateTime.UtcNow.AddDays(-1);
            var quizId = Guid.NewGuid();
            var sectionId1 = Guid.NewGuid();
            var sectionId2 = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();

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
                        Id = sectionId1,
                        QuizId = quizId,
                        Description = "section 1",
                        Index = 1,
                        Created = lastQuizUpdateAt,
                        Updated = lastQuizUpdateAt,
                        Questions = new List<SCModels.Question>
                        {
                            new SCModels.Question {
                                Id = questionId,
                                QuizSectionId = sectionId1,
                                QuizSectionIndex = 1,
                                Header = "header 1",
                                Body = "body",
                                Footer = "footer",
                                Created = lastQuizUpdateAt,
                                Updated = lastQuizUpdateAt
                            },
                            new SCModels.Question {
                                Id = questionId2,
                                QuizSectionId = sectionId1,
                                QuizSectionIndex = 2,
                                Header = "header 2",
                                Body = "body 2",
                                Footer = "footer 2",
                                Created = lastQuizUpdateAt,
                                Updated = lastQuizUpdateAt
                            }
                        }
                    },
                    new SCModels.QuizSection
                    {
                        Id = sectionId2,
                        QuizId = quizId,
                        Description = "section 2",
                        Index = 1,
                        Created = lastQuizUpdateAt,
                        Updated = lastQuizUpdateAt,
                    }
                }
            };

            var payload = new SCModels.Question()
            {
                QuizSectionId = sectionId2,
                QuizSectionIndex = 1,
                Body = "test question body"
            };

            using var context = GetDbContext();
            await Seed(new List<SCModels.Quiz>() { quiz });
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var updatedId = await adminQuestionService.UpdateQuestionAsync(questionId, payload);

            updatedId.ShouldBe(questionId);
            var updatedQuestion = await context.Questions.FindAsync(updatedId);
            updatedQuestion.ShouldNotBeNull();
            updatedQuestion.Body.ShouldBe("test question body");
            updatedQuestion.QuizSectionId.ShouldBe(sectionId2);
            updatedQuestion.Updated.Value.ShouldNotBe(updatedQuestion.Created);

            var section2questionCount = await context.Questions.CountAsync(q => q.QuizSectionId == sectionId2);
            section2questionCount.ShouldBe(1);

            var section1questionCount = await context.Questions.CountAsync(q => q.QuizSectionId == sectionId1);
            section1questionCount.ShouldBe(1);

            var sibling = await context.Questions.FindAsync(questionId2);
            sibling.QuizSectionIndex.ShouldBe(1);

            var updatedQuiz = await context.Quizzes.FindAsync(quizId);
            updatedQuiz.Updated.Value.ShouldNotBe(lastQuizUpdateAt);

        }

        [Fact]
        public async Task ValidPayload_PositionSwapped_Should_UpdateQuestion()
        {
            var lastQuizUpdateAt = DateTime.UtcNow.AddDays(-1);
            var quizId = Guid.NewGuid();
            var sectionId1 = Guid.NewGuid();
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
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
                        Id = sectionId1,
                        QuizId = quizId,
                        Description = "section 1",
                        Index = 1,
                        Created = lastQuizUpdateAt,
                        Updated = lastQuizUpdateAt,
                        Questions = new List<SCModels.Question>
                        {
                            new SCModels.Question {
                                Id = questionId1,
                                QuizSectionId = sectionId1,
                                QuizSectionIndex = 1,
                                Header = "header 1",
                                Body = "body",
                                Footer = "footer",
                                Created = lastQuizUpdateAt,
                                Updated = lastQuizUpdateAt
                            },
                            new SCModels.Question {
                                Id = questionId2,
                                QuizSectionId = sectionId1,
                                QuizSectionIndex = 2,
                                Header = "header 2",
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
                QuizSectionId = sectionId1,
                QuizSectionIndex = 1,
                Body = "test question body"
            };

            using var context = GetDbContext();
            await Seed(new List<SCModels.Quiz>() { quiz });
            await context.SaveChangesAsync();

            var adminQuestionService = new AdminQuizQuestionService(context);
            var updatedId = await adminQuestionService.UpdateQuestionAsync(questionId2, payload);

            updatedId.ShouldBe(questionId2);
            var updatedQuestion = await context.Questions.FindAsync(updatedId);
            updatedQuestion.ShouldNotBeNull();
            updatedQuestion.Body.ShouldBe("test question body");
            updatedQuestion.QuizSectionId.ShouldBe(sectionId1);
            updatedQuestion.Updated.Value.ShouldNotBe(updatedQuestion.Created);

            var updatedQuiz = await context.Quizzes.FindAsync(quizId);
            updatedQuiz.Updated.Value.ShouldNotBe(lastQuizUpdateAt);
        }

    }
}
