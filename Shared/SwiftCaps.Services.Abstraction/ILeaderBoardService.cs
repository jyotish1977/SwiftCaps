using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface ILeaderBoardService
    {
        Task<ServiceResponse<IList<LeaderBoard>>> GetLeaderBoard(UserQuizRequest userQuizRequest);
    }
}
