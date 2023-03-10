

using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using Xamariners.RestClient.Helpers.Models;
using Xamariners.RestClient.Infrastructure;

namespace SwiftCaps.Client.Core.Services
{
    public class LeaderBoardService : ServiceBase, ILeaderBoardService
    {
        private const string API_NAME = "reporting";

        public async Task<ServiceResponse<IList<LeaderBoard>>> GetLeaderBoard(UserQuizRequest userQuizRequest)
        {
            var response = await RestClient
                .ExecuteAsync<IList<LeaderBoard>, UserQuizRequest>(
                    HttpVerb.POST,
                    $"{nameof(GetLeaderBoard)}".ToLower(),
                    isServiceResponse: true,
                    paramMode: HttpParamMode.BODY,
                    requestBody: userQuizRequest,
                    apiRoutePrefix: API_NAME,
                    headers: AppSettings.Headers).ConfigureAwait(false);

            return response;
        }
    }
}
