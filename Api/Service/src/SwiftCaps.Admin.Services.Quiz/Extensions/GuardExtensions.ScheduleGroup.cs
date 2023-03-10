using System;
using Ardalis.GuardClauses;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        public static void InvalidScheduleGroupListPayload(this IGuardClause guardClause,
                                                           Guid scheduleId)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
        }

        public static void InvalidScheduleGroupCreatePayload(this IGuardClause guardClause,
                                                             Guid scheduleId,
                                                             Group group)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
            Guard.Against.Null(group, "group", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(group.Id, "group id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrWhiteSpace(group.Name, "group id", MissingInvalidPayloadMessage);
        }

        public static void InvalidScheduleGroupDeletePayload(this IGuardClause guardClause,
                                                             Guid scheduleId,
                                                             Guid groupId)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(groupId, "group id", MissingInvalidPayloadMessage);
        }

        public static void InvalidScheduleGroupSearchPayload(this IGuardClause guardClause,
                                                             Guid scheduleId)
        {
            Guard.Against.NullOrEmpty(scheduleId, "schedule id", MissingInvalidPayloadMessage);
        }
    }
}
