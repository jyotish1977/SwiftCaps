using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Web.Shared.Clients
{
    public interface IReportingClient
    {
        [Post("/reporting/getleaderboard")]
        Task<ServiceResponse<IList<LeaderBoard>>> GetLeaderboard(UserQuizRequest userQuizRequest);
    }
}
