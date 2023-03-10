using Ardalis.GuardClauses;
using SwiftCaps.Models.Requests;

namespace SwiftCaps.Services.Reporting.Extensions
{
    public static class GuardExtensions
    {
        public static void InvalidLeaderBoardReadPayload(this IGuardClause guardClause, UserQuizRequest userQuizRequest, bool requiresUser = true)
        {
            Guard.Against.Null(userQuizRequest, "user quiz request", "Missing payload or invalid payload provided.");
            if(requiresUser)
            {
                Guard.Against.NullOrEmpty(userQuizRequest.UserId, "user id", "Missing payload or invalid payload provided.");
            }
            Guard.Against.InvalidInput(userQuizRequest.ClientLocalDateTime, "client local datetime",
                                        clientdate => clientdate != default, "Missing payload or invalid payload provided.");
        }
    }
}
