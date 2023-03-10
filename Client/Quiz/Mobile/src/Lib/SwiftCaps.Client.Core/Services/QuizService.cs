using System;
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
    public class QuizService : ServiceBase, IQuizService
    {
        public QuizService()
        {

        }


        public async Task<ServiceResponse<IList<UserQuiz>>> GetAvailableUserQuizzes(UserQuizRequest userQuizRequest)
        {
            // rest client call
            var response = await RestClient
                .ExecuteAsync<IList<UserQuiz>, UserQuizRequest>(
                    HttpVerb.POST,
                    nameof(GetAvailableUserQuizzes),
                    isServiceResponse: true,
                    paramMode: HttpParamMode.BODY,
                    requestBody: userQuizRequest,
                    apiRoutePrefix: nameof(UserQuiz),
                    headers: AppSettings.Headers)
                .ConfigureAwait(false);

            return response;
        }

        public async Task<ServiceResponse<UserQuiz>> AddUserQuiz(UserQuiz userQuiz)
        {
            var response = await RestClient
                .ExecuteAsync<UserQuiz>(
                    HttpVerb.POST,
                    nameof(AddUserQuiz),
                    isServiceResponse: true,
                    paramMode: HttpParamMode.BODY,
                    requestBody: userQuiz,
                    apiRoutePrefix: nameof(UserQuiz),
                    headers: AppSettings.Headers)
                .ConfigureAwait(false);

            return response;
        }

        public async Task<ServiceResponse<bool>> SaveUserQuiz(UserQuiz userQuiz)
        {
            var response = await RestClient
                .ExecuteAsync<bool, UserQuiz>(
                    HttpVerb.POST,
                    nameof(SaveUserQuiz),
                    isServiceResponse: true,
                    paramMode: HttpParamMode.BODY,
                    requestBody: userQuiz,
                    apiRoutePrefix: nameof(UserQuiz),
                    headers: AppSettings.Headers)
                .ConfigureAwait(false);

            return response;
        }
    }
}
