using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Abstraction.Interfaces
{
    public interface IAdminQuizSectionService
    {
        Task<IList<QuizSection>> GetSectionsAsync(Guid quizId);
        Task<QuizSection> GetSectionAsync(Guid sectionId);
        Task<Guid?> CreateSectionAsync(QuizSection newSection);
        Task<Guid?> UpdateSectionAsync(Guid sectionId, QuizSection quizSection);
        Task<bool> DeleteSectionAsync(Guid sectionId);
    }
}


