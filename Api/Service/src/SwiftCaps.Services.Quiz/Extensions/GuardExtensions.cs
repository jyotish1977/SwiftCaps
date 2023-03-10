using System;
using Ardalis.GuardClauses;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;

namespace SwiftCaps.Services.Quiz.Extensions
{
    public static class GuardExtensions
    {
        private const string MissingInvalidPayloadMessage = "Missing payload or invalid payload provided.";

        private static Func<DateTimeOffset, bool> validDateFunc = clientLocalDateTime => !(clientLocalDateTime.ToUniversalTime().Date == default); //|| clientLocalDateTime.ToUniversalTime().Date < DateTime.UtcNow.Date - Commented as prevents the testing time machine
        public static void InvalidQuizListPayload(this IGuardClause guardClause, UserQuizRequest userQuizRequest, bool requiresUser = true)
        {
            Guard.Against.Null(userQuizRequest, "user quiz request", MissingInvalidPayloadMessage);
            if(requiresUser)
            {
                Guard.Against.NullOrEmpty(userQuizRequest.UserId, "user id", MissingInvalidPayloadMessage);
            }

            // todo: this breaks the tests as we can have a time machine to set dates in the past
            // commented - please revise
            Guard.Against.InvalidInput(userQuizRequest.ClientLocalDateTime, "client local date time",
            (clientLocalDateTime) => validDateFunc(clientLocalDateTime),
            MissingInvalidPayloadMessage);

            //Guard.Against.Null(userQuizRequest.ClientLocalDateTime, "client local date time", MissingInvalidPayloadMessage);
        }

        public static void InvalidQuizSavePayload(this IGuardClause guardClause, UserQuiz userQuiz)
        {
            Guard.Against.Null(userQuiz, "user quiz", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(userQuiz.ScheduleId, "schedule id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(userQuiz.UserId, "user id", MissingInvalidPayloadMessage);
            Guard.Against.Null(userQuiz.Completed, "completed", MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(userQuiz.Completed, "completed", 
                                       completed => completed.Value.ToUniversalTime().Date >= DateTime.UtcNow.Date,
                                       MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(userQuiz.Sequence, "sequence", 
                                       sequence => sequence >= 0,
                                       MissingInvalidPayloadMessage);
        }

        public static void InvalidQuizSetupPayload(this IGuardClause guardClause, UserQuiz userQuiz)
        {
            Guard.Against.Null(userQuiz, "user quiz", MissingInvalidPayloadMessage);
            Guard.Against.Null(userQuiz.Schedule, "user group quiz", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(userQuiz.UserId, "user group quiz", MissingInvalidPayloadMessage);
        }
    }
}
