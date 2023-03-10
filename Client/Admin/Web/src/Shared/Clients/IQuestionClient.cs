using System;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IQuestionClient
    {
        [Get("/admin/quiz/question/{questionId}")]
        Task<ServiceResponse<Question>> GetQuestion(Guid questionId);

        [Post("/admin/quiz/question")]
        Task<ServiceResponse<Guid?>> AddQuestion(Question quizQuestion);

        [Put("/admin/quiz/question/{questionId}")]
        Task<ServiceResponse<Guid?>> UpdateQuestion(Guid questionId, Question quizQuestion);

        [Delete("/admin/quiz/question/{questionId}")]
        Task<ApiResponse<ServiceResponse<bool>>> DeleteQuestion(Guid questionId);
    }
}
