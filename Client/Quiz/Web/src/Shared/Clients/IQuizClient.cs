using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Web.Shared.Clients
{
    public interface IQuizClient
    {
        [Post("/userquiz/getavailableuserquizzes")]
        Task<ServiceResponse<IList<UserQuiz>>> GetAvailableUserQuizzes(UserQuizRequest userQuizRequest);

        [Post("/userquiz/adduserquiz")]
        Task<ServiceResponse<UserQuiz>> AddUserQuiz(UserQuiz userQuiz);

        [Post("/userquiz/saveuserquiz")]
        Task<ServiceResponse<bool>> SaveUserQuiz(UserQuiz userQuiz);
    }
}
