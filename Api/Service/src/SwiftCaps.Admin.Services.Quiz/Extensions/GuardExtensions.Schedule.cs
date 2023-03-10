using System;
using Ardalis.GuardClauses;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        private static readonly Func<DateTime?,bool> currentAndFutureDateCheck = date => date?.ToUniversalTime().Date >= DateTime.UtcNow.Date;

        public static void InvalidScheduleCreatePayload(this IGuardClause guardClause, Schedule schedule)
        {
            Guard.Against.Null(schedule, "Schedule", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(schedule.QuizId, "quiz id", MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(schedule.StartTime, "schedule start", currentAndFutureDateCheck, MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(schedule.EndTime, "schedule end", currentAndFutureDateCheck, MissingInvalidPayloadMessage);
        }

        public static void InvalidScheduleReadPayload(this IGuardClause guardClause, Guid scheduleId)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
        }
        
        public static void InvalidScheduleUpdatePayload(this IGuardClause guardClause, Guid scheduleId, Schedule schedule)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
            Guard.Against.Null(schedule, "Schedule", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(schedule.QuizId, "quiz id", MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(schedule.StartTime, "schedule start", date => date.HasValue , MissingInvalidPayloadMessage);
            Guard.Against.InvalidInput(schedule.EndTime, "schedule end", date => date.HasValue, MissingInvalidPayloadMessage);
        }

        public static void InvalidScheduleDeletePayload(this IGuardClause guardClause, Guid scheduleId)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
        }
    }
}
