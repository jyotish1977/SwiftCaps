using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SwiftCaps.Client.Cache.Service.Data;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Helpers.DateTime.Interfaces;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Infrastructure;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Client.Cache.Service.Services
{
    public class QuizCacheService
    {
        private readonly ISwiftCapsApiServices _services;
        public IDateTimeOffsetProvider DateTimeOffsetProvider { get; }

        public QuizCacheService(ISwiftCapsApiServices services, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _services = services;
            DateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        internal async Task<ServiceResponse<bool>> ClearUserQuizCache()
        {
            // brute clear data and repopulate
            using (var dbContext = new SwiftCapsClientContext(_services.AppCache.State.AppDataPath))
            {
                if (!dbContext.UserQuizzes.Any())
                    return new ServiceResponse<bool>(ServiceStatus.Success);

                try
                {
                    dbContext.ScheduleGroups.RemoveRange(dbContext.ScheduleGroups.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    dbContext.Questions.RemoveRange(dbContext.Questions.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    dbContext.QuizSections.RemoveRange(dbContext.QuizSections.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    dbContext.Quizzes.RemoveRange(dbContext.Quizzes.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    dbContext.UserQuizzes.RemoveRange(dbContext.UserQuizzes.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    dbContext.Groups.RemoveRange(dbContext.Groups.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>(false, "Error Clearing UserQuiz Cache", ex.Message, ex);
                }
            }

            return new ServiceResponse<bool>(ServiceStatus.Success);
        }

        internal async Task<ServiceResponse<bool>> SetUserQuizCache(string appDataPath, Guid userId)
        {
            // get userquizzes
            var response = await _services.QuizService
                .GetAvailableUserQuizzes(new UserQuizRequest
                {
                    UserId = userId, ClientLocalDateTime = DateTimeOffsetProvider.Now
                })
                .ConfigureAwait(false);

            if (!response.IsOK())
                return response.ToServiceResponse<IList<UserQuiz>, bool>();

            // populate userquizzes
            for (var i = 0; i < response.Data.Count; i++)
            {
                var userQuizResponse =
                    await _services.QuizService.AddUserQuiz(response.Data[i]).ConfigureAwait(false);

                if (!userQuizResponse.IsOK())
                    return response.ToServiceResponse<IList<UserQuiz>, bool>();

                response.Data[i] = userQuizResponse.Data;
            }

            // repopulate cache
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                try
                {
                    var quizzes = response.Data.Select(x => x.Schedule.Quiz).ToList().DistinctBy(x => x.Id);
                    var schedules = response.Data.Select(x => x.Schedule).ToList();
                    schedules.ForEach(x => x.Quiz = null);
                    response.Data.ForEach(x => x.Schedule = null);
                    await dbContext.AddRangeAsync(quizzes).ConfigureAwait(false);
                    await dbContext.AddRangeAsync(schedules).ConfigureAwait(false);
                    await dbContext.AddRangeAsync(response.Data).ConfigureAwait(false);
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>(false, "Error Setting UserQuizzes Cache", ex.Message, ex);
                }
            }

            return new ServiceResponse<bool>(ServiceStatus.Success);
        }

        public async Task<ServiceResponse<List<UserQuiz>>> GetUserQuizzesCache(string appDataPath)
        {
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                var quizzes = await dbContext.UserQuizzes
                    .Include(x => x.Schedule)
                    .ThenInclude(x => x.Quiz)
                    .OrderBy(x => x.Expiry)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var quiz in quizzes)
                {
                    quiz.Schedule.Quiz.QuizSections = await dbContext.QuizSections
                        .Include(qs => qs.Questions.OrderBy(q => q.QuizSectionIndex))
                        .OrderBy(qs => qs.Index)
                        .Where(qs => qs.QuizId == quiz.Schedule.Quiz.Id)
                        .ToListAsync();
                }

                return quizzes.AsServiceResponse();
            }
        }

        public async Task<ServiceResponse<UserQuiz>> GetUserQuizCache(string appDataPath, Guid id)
        {
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                var result = await dbContext.UserQuizzes
                    .Include(x => x.Schedule)
                    .ThenInclude(x => x.Quiz)
                    .ThenInclude(x => x.QuizSections.OrderBy(qs => qs.Index))
                    .ThenInclude(x => x.Questions.OrderBy(q => q.QuizSectionIndex))
                    .ThenInclude(x => x.QuizAnswers.OrderBy(qa => qa.AnswerIndex))
                    .FirstOrDefaultAsync(x => x.Id == id)
                    .ConfigureAwait(false);

                return result.AsServiceResponse();
            }
        }

        public async Task<ServiceResponse<bool>> SaveUserQuiz(string appDataPath, UserQuiz userQuiz)
        {
            var response = await _services.QuizService.SaveUserQuiz(userQuiz).ConfigureAwait(false);

            if (response.HttpStatus != HttpStatusCode.Created && response.HttpStatus != HttpStatusCode.OK)
                return response;

            // save on api and locally
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                var cachedUserQuiz = await dbContext.UserQuizzes.FirstOrDefaultAsync(x =>
                    x.ScheduleId == userQuiz.ScheduleId &&
                    x.Sequence == userQuiz.Sequence &&
                    x.UserId == userQuiz.UserId);
                if (cachedUserQuiz != null)
                {
                    cachedUserQuiz.Completed = userQuiz.Completed;
                    userQuiz = cachedUserQuiz;
                }

                dbContext.UserQuizzes.Update(userQuiz);

                try
                {
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>(false, "Error Saving UserQuizzes Cache", ex.Message, ex);
                }
            }

            return response;
        }
    }
}