using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SwiftCaps.Models.Models;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCAPS.Admin.Web.Shared.Clients
{
    public interface IAdminQuizSectionClient
    {
        [Get("/admin/quiz/{quizId}/section")]
        Task<ServiceResponse<IList<QuizSection>>> GetQuizSections(Guid quizId);

        [Get("/admin/quiz/section/{sectionId}")]
        Task<ServiceResponse<QuizSection>> GetQuizSection(Guid sectionId);

        [Post("/admin/quiz/section")]
        Task<ServiceResponse<Guid?>> AddQuizSection(QuizSection quizSection);

        [Put("/admin/quiz/section/{sectionId}")]
        Task<ServiceResponse<Guid?>> UpdateQuizSection(Guid sectionId, QuizSection quizSection);

        [Delete("/admin/quiz/section/{sectionId}")]
        Task<ApiResponse<ServiceResponse<bool>>> DeleteQuizSection(Guid sectionId);
    }
}
