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
    public class AdminQuizQuestionService : IAdminQuizQuestionService
    {

        private readonly SwiftCapsContext _context;

        public AdminQuizQuestionService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<SCModels.Question> GetQuestionAsync(Guid questionId)
        {
            Guard.Against.InvalidQuestionReadPayload(questionId);

            var question = await _context.Questions
                                         .SingleOrDefaultAsync(q => q.Id == questionId);
            if(question == null)
            {
                throw new NotFoundException(questionId.ToString(), "Quiz");
            }
            
            return question;
        }


        public async Task<Guid?> CreateQuestionAsync(SCModels.Question question)
        {
            Guard.Against.InvalidQuestionCreatePayload(question);

            try
            {
                var section = _context.QuizSections
                                      .Include(s => s.Questions)
                                      .SingleOrDefault(s => s.Id == question.QuizSectionId);
                if(section == null)
                {
                    throw new NotFoundException(question.QuizSectionId.ToString(), "Section");
                }

                DateTime updateTimeStamp = DateTime.UtcNow;

                question.QuizSectionIndex = section.Questions.Count + 1;
                question.Body = question.Body.Trim();
                question.Description = question.Description?.Trim();
                question.Header = question.Header?.Trim();
                question.Footer = question.Footer?.Trim();
                question.Updated = question.Created = updateTimeStamp;
                var resultAdd = await _context.Questions.AddAsync(question);

                var quiz = _context.Quizzes.SingleOrDefault(q => q.Id == section.QuizId);
                quiz.Updated = updateTimeStamp;
                _context.Update(quiz);

                await _context.SaveChangesAsync();
                return resultAdd.Entity.Id;
            }
            catch(Exception exp)
            {
                throw exp;
            }
        }

        
        public async Task<Guid?> UpdateQuestionAsync(Guid questionId, SCModels.Question payload)
        {
            Guard.Against.InvalidQuestionUpdatePayload(questionId, payload);

            try 
            { 
                var questionToUpdate = await _context.Questions.SingleOrDefaultAsync(q => q.Id == questionId);
                if (questionToUpdate == null)
                {
                    throw new NotFoundException(questionId.ToString(), "Question");
                }

                DateTime updateTimeStamp = DateTime.UtcNow;

                var swapSection = !questionToUpdate.QuizSectionId.Equals(payload.QuizSectionId);
                var swapQuestionIndex = questionToUpdate.QuizSectionId.Equals(payload.QuizSectionId) 
                                        && questionToUpdate.QuizSectionIndex != payload.QuizSectionIndex;

                if(swapSection)
                {
                    var newSectionQuestionCount = await _context.Questions.CountAsync(q => q.QuizSectionId == payload.QuizSectionId);

                    var sourceSectionSiblings = _context.Questions.Where(q => q.QuizSectionId == questionToUpdate.QuizSectionId
                                                                              && q.QuizSectionIndex > questionToUpdate.QuizSectionIndex);
                    foreach(var sibling in sourceSectionSiblings)
                    {
                         sibling.QuizSectionIndex = sibling.QuizSectionIndex - 1;
                        _context.Update(sibling);
                    }

                    questionToUpdate.QuizSectionId = payload.QuizSectionId;
                    questionToUpdate.QuizSectionIndex = ++newSectionQuestionCount;
                }
                if(swapQuestionIndex)
                {
                    var sibling = await _context.Questions.SingleOrDefaultAsync(q => q.QuizSectionId == questionToUpdate.QuizSectionId
                                                                                     && q.QuizSectionIndex == payload.QuizSectionIndex);
                    sibling.QuizSectionIndex = questionToUpdate.QuizSectionIndex;
                    sibling.Updated = updateTimeStamp;
                    _context.Update(sibling);
                    questionToUpdate.QuizSectionIndex = payload.QuizSectionIndex;
                }
                questionToUpdate.Body = payload.Body.Trim();
                questionToUpdate.Description = payload.Description?.Trim();
                questionToUpdate.Header = payload.Header?.Trim();
                questionToUpdate.Footer = payload.Footer?.Trim();
                questionToUpdate.Updated = updateTimeStamp;
                _context.Update(questionToUpdate);

                var section = _context.QuizSections
                                      .SingleOrDefault(s => s.Id == questionToUpdate.QuizSectionId);
                var quiz = _context.Quizzes.SingleOrDefault(q => q.Id == section.QuizId);
                quiz.Updated = updateTimeStamp;
                _context.Update(quiz);

                await _context.SaveChangesAsync();

                return questionToUpdate.Id;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        public async Task<bool> DeleteQuestionAsync(Guid questionId)
        {
            Guard.Against.InvalidQuestionDeletePayload(questionId);

            try
            {
                var questionToDelete = _context.Questions.SingleOrDefault(q => q.Id == questionId);
                if (questionToDelete == null)
                {
                    throw new NotFoundException(questionId.ToString(), "Question");
                }

                DateTime updateTimeStamp = DateTime.UtcNow;

                var siblings = await _context.Questions.Where(q => q.QuizSectionId == questionToDelete.QuizSectionId 
                                                        && q.QuizSectionIndex > questionToDelete.QuizSectionIndex)
                                                       .ToListAsync();

                _context.Questions.Remove(questionToDelete);

                foreach(var sibling in siblings)
                {
                    sibling.QuizSectionIndex = sibling.QuizSectionIndex - 1;
                    _context.Update(sibling);
                }


                var section = _context.QuizSections
                                      .SingleOrDefault(s => s.Id == questionToDelete.QuizSectionId);
                var quiz = _context.Quizzes.SingleOrDefault(q => q.Id == section.QuizId);
                quiz.Updated = updateTimeStamp;
                _context.Update(quiz);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

    }
}
