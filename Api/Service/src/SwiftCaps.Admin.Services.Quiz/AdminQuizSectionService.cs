using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Models;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Admin.Services
{
    public class AdminQuizSectionService : IAdminQuizSectionService
    {
        private readonly SwiftCapsContext _context;

        public AdminQuizSectionService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<IList<QuizSection>> GetSectionsAsync(Guid quizId)
        {
            Guard.Against.InvalidSectionListPayload(quizId);

            var quizExists = await _context.Quizzes.AnyAsync(Q => Q.Id == quizId);
            if (!quizExists)
            {
                throw new NotFoundException(quizId.ToString(), "Quiz");
            }

            var sections = await _context.QuizSections
                                   .Where(q => q.QuizId == quizId)
                                   .OrderBy(s => s.Index)
                                   .ToListAsync();

            return sections;
        }

        public async Task<QuizSection> GetSectionAsync(Guid sectionId)
        {
            Guard.Against.InvalidSectionReadPayload(sectionId);

            var section = await _context.QuizSections
                                        .Include(section  => section.Questions)
                                        .SingleOrDefaultAsync(s => s.Id == sectionId);
            
            if(section == null)
            {
                throw new NotFoundException(sectionId.ToString(), "Section");
            }
            
            section.Questions = section.Questions.OrderBy(q => q.QuizSectionIndex).ToList();

            return section;
        }

        public async Task<Guid?> CreateSectionAsync(QuizSection newSection)
        {
            Guard.Against.InvalidSectionCreatePayload(newSection);

            var quiz = await _context.Quizzes
                                     .Include(quiz => quiz.QuizSections) 
                                     .SingleOrDefaultAsync(Q => Q.Id == newSection.QuizId);
            if (quiz == null)
            {
                throw new NotFoundException(newSection.QuizId.ToString(), "Quiz");
            }

            newSection.Index = quiz.QuizSections.Count + 1;
            newSection.Updated = newSection.Created = DateTime.UtcNow;
            await _context.QuizSections.AddAsync(newSection);

            quiz.Updated = DateTime.UtcNow;
            _context.Update(quiz);

            await _context.SaveChangesAsync();
            return newSection.Id;
        }

        public async Task<Guid?> UpdateSectionAsync(Guid sectionId, QuizSection updatedSection)
        {
            Guard.Against.InvalidSectionUpdatePayload(sectionId, updatedSection);

            var sectionToUpdate = await _context.QuizSections.SingleOrDefaultAsync(Q => Q.Id == sectionId);
            if (sectionToUpdate == null)
            {
                throw new NotFoundException(sectionId.ToString(), "Section");
            }
            sectionToUpdate.Description = updatedSection.Description.Trim();
            sectionToUpdate.Updated = DateTime.UtcNow;
            _context.Update(sectionToUpdate);

            await UpdateQuizLastUpdated(sectionToUpdate.QuizId);

            await _context.SaveChangesAsync();
            return sectionToUpdate.Id;
        }

        public async Task<bool> DeleteSectionAsync(Guid sectionid)
        {
            Guard.Against.InvalidSectionDeletePayload(sectionid);

            var sectionToDelete = await _context.QuizSections.FindAsync(sectionid);
            if (sectionToDelete == null)
            {
                throw new NotFoundException(sectionid.ToString(), "Section");
            }
            
            var sectionToDeleteIndex = sectionToDelete.Index;
            var quizToUpdate = sectionToDelete.QuizId;

            _context.QuizSections.Remove(sectionToDelete);
            await _context.SaveChangesAsync();

            await UpdateSiblingIndex(sectionToDeleteIndex);
            await UpdateQuizLastUpdated(quizToUpdate);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task UpdateQuizLastUpdated(Guid quizToUpdate)
        {
            var quiz = await _context.Quizzes.SingleOrDefaultAsync(q => q.Id == quizToUpdate);
            quiz.Updated = DateTime.UtcNow;
            _context.Update(quiz);
        }

        private async Task UpdateSiblingIndex(int sectionToDeleteIndex)
        {
            var siblings = await _context.QuizSections
                                                      .Where(s => s.Index > sectionToDeleteIndex)
                                                      .OrderBy(s => s.Index)
                                                      .ToListAsync();
            siblings.ForEach(sibling => sibling.Index--);
            _context.UpdateRange(siblings);
        }
    }
}
