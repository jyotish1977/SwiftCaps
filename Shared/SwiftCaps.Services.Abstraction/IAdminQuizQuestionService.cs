using System;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IAdminQuizQuestionService
    {
        Task<Question> GetQuestionAsync(Guid questionId);
        Task<Guid?> CreateQuestionAsync(Question question);
        Task<Guid?> UpdateQuestionAsync(Guid questionId, Question question);
        Task<bool> DeleteQuestionAsync(Guid questionId);
    }
}
