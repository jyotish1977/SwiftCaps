using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IQuizService
    {
        Task<ServiceResponse<IList<UserQuiz>>> GetAvailableUserQuizzes(UserQuizRequest userQuizRequest);
        Task<ServiceResponse<UserQuiz>> AddUserQuiz(UserQuiz userQuiz);
        Task<ServiceResponse<bool>> SaveUserQuiz(UserQuiz userQuiz);
    }
}
