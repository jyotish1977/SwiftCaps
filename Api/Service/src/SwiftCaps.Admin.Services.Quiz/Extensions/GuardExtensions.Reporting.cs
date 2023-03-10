using System;
using Ardalis.GuardClauses;
using SwiftCaps.Models.Requests;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static partial class GuardExtensions
    {
        public static void InvalidReportingLeaderboardPayload(this IGuardClause guardClause,
                                                              AdminReportingRequest request)
        {
            Guard.Against.Null(request, "leaderboard request", MissingInvalidPayloadMessage);
            Guard.Against.NullOrEmpty(request.GroupId, "group id", MissingInvalidPayloadMessage);
        }
    }
}
