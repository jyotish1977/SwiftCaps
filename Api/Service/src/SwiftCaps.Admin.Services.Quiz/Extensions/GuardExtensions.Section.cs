using System;
using Ardalis.GuardClauses;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        public static void InvalidSectionListPayload(this IGuardClause guardClause, Guid quizId)
        {
            Guard.Against.NullOrEmpty(quizId,"quiz id", MissingInvalidPayloadMessage);
        }

        public static void InvalidSectionReadPayload(this IGuardClause guardClause, Guid sectionId)
        {
            Guard.Against.NullOrEmpty(sectionId,"section id", MissingInvalidPayloadMessage);
        }

        public static void InvalidSectionCreatePayload(this IGuardClause guardClause, SCModels.QuizSection section)
        {
            Guard.Against.Null(section,"section", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(section.QuizId,"quiz id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(section.Description,"description", MissingInvalidPayloadMessage);
        }

        public static void InvalidSectionUpdatePayload(this IGuardClause guardClause, Guid sectionId, SCModels.QuizSection section)
        {
            Guard.Against.NullOrEmpty(sectionId,"section id", MissingInvalidPayloadMessage);
            Guard.Against.Null(section,"section", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(section.Description,"description", MissingInvalidPayloadMessage);
        }

        public static void InvalidSectionDeletePayload(this IGuardClause guardClause, Guid sectionId)
        {
            Guard.Against.NullOrEmpty(sectionId,"section id", MissingInvalidPayloadMessage);
        }
    }
}
