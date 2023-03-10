using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SwiftCaps.Admin.Services.Quiz.Extensions;
using SwiftCaps.Data.Context;
using SwiftCaps.Services.Abstraction.Interfaces;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services
{
    public class AdminQuizService : IAdminQuizService
    {
        private readonly SwiftCapsContext _context;

        public AdminQuizService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<IList<SCModels.QuizSummary>> GetQuizzesAsync()
        {
            var result = new List<SCModels.QuizSummary>();

            var quizzes = await _context.Quizzes
                                .Include(q => q.QuizSections)
                                .ThenInclude(qs => qs.Questions)
                                .Include(q => q.Schedules)
                                .OrderBy(q => q.Name)
                                .ToListAsync();


            foreach (var quiz in quizzes)
            {
                result.Add(new SCModels.QuizSummary
                {
                    Id = quiz.Id,
                    LastUpdated = quiz.Updated,
                    Questions = quiz.QuizSections?.Sum(x => x.Questions.Count) ?? 0,
                    Sections = quiz.QuizSections?.Count ?? 0,
                    Title = quiz.Name,
                    Schedules = quiz.Schedules.Count
                });
            }

            return result;
        }

        public async Task<SCModels.Quiz> GetQuizAsync(Guid quizId)
        {
            Guard.Against.InvalidQuizReadPayload(quizId);

            var quiz = await _context.Quizzes
                                    .Include(x => x.QuizSections)
                                    .ThenInclude(x => x.Questions)
                                    .SingleOrDefaultAsync(x => x.Id == quizId);

            if (quiz == null)
            {
                throw new NotFoundException(quizId.ToString(), "Quiz");
            }

            quiz.QuizSections = quiz.QuizSections.OrderBy(s => s.Index).ToList();

            quiz.QuizSections.ForEach(sections =>
            {
                sections.Questions = sections.Questions.OrderBy(x => x.QuizSectionIndex).ToList();
            });

            return quiz;
        }

        public async Task<Guid?> CreateQuizAsync(SCModels.Quiz quiz)
        {
            Guard.Against.InvalidQuizCreatePayload(quiz);

            try
            {
                quiz.Name = quiz.Name.Trim();
                quiz.Description = quiz.Description.Trim();
                quiz.InfoMarkdown = quiz.InfoMarkdown?.Trim();
                quiz.Updated = quiz.Created = DateTime.UtcNow;

                var resultAdd = await _context.Quizzes.AddAsync(quiz);
                await _context.SaveChangesAsync();
                return resultAdd.Entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Guid?> UpdateQuizAsync(Guid quizId, SCModels.Quiz quiz)
        {
            Guard.Against.InvalidQuizUpdatePayload(quizId, quiz);

            try
            {
                var quizToUpdate = await _context.Quizzes.SingleOrDefaultAsync(x => x.Id == quizId);
                if (quizToUpdate == null)
                {
                    throw new NotFoundException(quizId.ToString(), "Quiz");
                }

                _context.Entry(quizToUpdate).State = EntityState.Modified;

                quizToUpdate.Name = quiz.Name.Trim();
                quizToUpdate.Description = quiz.Description.Trim();
                quizToUpdate.InfoMarkdown = quiz.InfoMarkdown?.Trim();
                quizToUpdate.Updated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return quizToUpdate.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteQuizAsync(Guid quizId)
        {
            Guard.Against.InvalidQuizReadPayload(quizId);

            try
            {
                var quizToDelete = _context.Quizzes.SingleOrDefault(x => x.Id == quizId);
                if (quizToDelete == null)
                {
                    throw new NotFoundException(quizId.ToString(), "Quiz");
                }
                _context.Quizzes.Remove(quizToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
