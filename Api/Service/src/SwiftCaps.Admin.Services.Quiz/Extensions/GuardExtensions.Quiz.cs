using System;
using Ardalis.GuardClauses;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        public static void InvalidQuizReadPayload(this IGuardClause guardClause, Guid quizId)
        {
            Guard.Against.NullOrEmpty(quizId, "quiz id", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuizCreatePayload(this IGuardClause guardClause, SCModels.Quiz quiz)
        {
            Guard.Against.Null(quiz, "Quiz", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(quiz.Name, "quiz name", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(quiz.Description, "quiz description", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuizUpdatePayload(this IGuardClause guardClause, Guid quizId, SCModels.Quiz quiz)
        {
            Guard.Against.NullOrEmpty(quizId, "quiz id", MissingInvalidPayloadMessage);
            Guard.Against.Null(quiz, "Quiz", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(quiz.Name, "quiz name", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(quiz.Description, "quiz description", MissingInvalidPayloadMessage);
        }
    }
}
