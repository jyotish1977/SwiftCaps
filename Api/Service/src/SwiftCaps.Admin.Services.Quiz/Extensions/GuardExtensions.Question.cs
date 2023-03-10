using System;
using Ardalis.GuardClauses;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        public static void InvalidQuestionReadPayload(
            this IGuardClause guardClause, Guid questionId)
        {
            Guard.Against.NullOrEmpty(questionId, "question id", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuestionDeletePayload(
            this IGuardClause guardClause, Guid questionId)
        {
            Guard.Against.NullOrEmpty(questionId, "question id", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuestionCreatePayload(
            this IGuardClause guardClause, SCModels.Question question)
        {
            Guard.Against.Null(question, "Question", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(question.QuizSectionId, "question section id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(question.Body, "question body", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuestionUpdatePayload(
            this IGuardClause guardClause, Guid questionId, SCModels.Question question)
        {
            Guard.Against.NullOrEmpty(questionId, "question id", MissingInvalidPayloadMessage);
            Guard.Against.Null(question, "Question", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(question.QuizSectionId, "question section id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(question.Body, "question body", MissingInvalidPayloadMessage);
        }
    }
}
