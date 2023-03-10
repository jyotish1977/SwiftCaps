using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IAdminQuizService
    {
        Task<IList<QuizSummary>> GetQuizzesAsync();
        Task<Quiz> GetQuizAsync(Guid quizId);
        Task<Guid?> CreateQuizAsync(Quiz quiz);
        Task<Guid?> UpdateQuizAsync(Guid quizId, Quiz quiz);
        Task<bool> DeleteQuizAsync(Guid quizId);
    }
}
