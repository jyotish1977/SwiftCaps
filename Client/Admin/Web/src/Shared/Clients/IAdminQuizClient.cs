using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IAdminQuizClient
    {
        [Get("/admin/quiz/list")]
        Task<ServiceResponse<IList<QuizSummary>>> GetQuizzes();

        [Get("/admin/quiz/{quizId}")]
        Task<ServiceResponse<Quiz>> GetQuiz(Guid quizId);

        [Post("/admin/quiz")]
        Task<ServiceResponse<Guid?>> AddQuiz(Quiz quiz);

        [Put("/admin/quiz/{quizId}")]
        Task<ServiceResponse<Guid?>> UpdateQuiz(Guid quizId, Quiz quiz);

        [Delete("/admin/quiz/{quizId}")]
        Task<ApiResponse<ServiceResponse<bool>>> DeleteQuiz(Guid quizId);
    }
}
