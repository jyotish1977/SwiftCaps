using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Helpers;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Quiz.Extensions;
using Xamariners.Core.Common.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;
using SCModels = SwiftCaps.Models.Models;

namespace SwiftCaps.Services.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly SwiftCapsContext _context;

        public QuizService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IList<UserQuiz>>> GetAvailableUserQuizzes(UserQuizRequest userQuizRequest)
        {
            Guard.Against.InvalidQuizListPayload(userQuizRequest);

            try
            {
                // get user
                var user = await _context.Users.FindAsync(userQuizRequest.UserId);
                if (user == null)
                    throw new NotFoundException(userQuizRequest.UserId.ToString(), "User");

                var monthShift = CalculateMonthShift(userQuizRequest);
                // check if is time to add a new weekly test to list
                var weekShift = CalculateWeekShift(userQuizRequest);

                // check if user already completed any test and the test has not expired
                var userQuizzes = GetCurrentUserCompletedNotExpiredQuizzes(userQuizRequest, user);

                var schedules = await GetCurrentUsersGroupSchedules(user);

                foreach (var schedule in schedules)
                {
                    schedule.Quiz.QuizSections = await GetQuizSections(schedule);

                    var expiry = schedule.Recurrence == Recurrence.Weekly
                                ? userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfWeek(false)
                                : userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfMonth(false);


                    if (!userQuizzes.Any(x => x.ScheduleId == schedule.Id && x.Expiry.Date == expiry.Date))
                    {
                        var newUserQuiz = GenerateUserQuiz(expiry,
                                                           user.Id,
                                                           schedule,
                                                           false);
                        userQuizzes.Add(newUserQuiz);
                    }

                    if (monthShift && schedule.Recurrence == Recurrence.Monthly)
                    {
                        var nextMonthItemToAdd = GenerateUserQuiz(userQuizRequest.ClientLocalDateTime.DateTime,
                                                                  user.Id,
                                                                  schedule,
                                                                  true);

                        // check if userQuizzes already have this item (in case the user already finished it already)
                        if (!userQuizzes.Any(d =>
                            d.ScheduleId == nextMonthItemToAdd.ScheduleId &&
                            d.Expiry.ToString("d") == nextMonthItemToAdd.Expiry.ToString("d")))
                        {
                            userQuizzes.Add(nextMonthItemToAdd);
                        }
                    }

                    if (weekShift && schedule.Recurrence == Recurrence.Weekly)
                    {
                        var nextWeekItemToAdd = GenerateUserQuiz(userQuizRequest.ClientLocalDateTime.DateTime,
                                                                 user.Id,
                                                                 schedule,
                                                                 true);

                        // check if userQuizzes already have this item (in case the user already finished it already)
                        if (!userQuizzes.Any(d => d.ScheduleId== nextWeekItemToAdd.ScheduleId &&
                                                  d.Expiry.ToString("d") == nextWeekItemToAdd.Expiry.ToString("d")))
                        {
                            userQuizzes.Add(nextWeekItemToAdd);
                        }
                    }

                    //// TODO: prettify
                    foreach (var qs in schedule.Quiz.QuizSections)
                    {
                        qs.Questions = qs.Questions.OrderBy(q => q.QuizSectionIndex).ToList();
                    }
                }

                foreach (var quiz in userQuizzes)
                    quiz.Schedule = schedules.FirstOrDefault(x => x.Id == quiz.ScheduleId);

                return userQuizzes.AsSuccessServiceResponse<IList<UserQuiz>>("Ok");
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> SaveUserQuiz(UserQuiz userQuiz)
        {
            Guard.Against.InvalidQuizSavePayload(userQuiz);

            try
            {
                var existingQuiz = await _context.UserQuizzes.FirstOrDefaultAsync(x =>
                    x.ScheduleId == userQuiz.ScheduleId &&
                    x.Sequence == userQuiz.Sequence &&
                    x.UserId == userQuiz.UserId);
                if (existingQuiz != null)
                {
                    throw new InvalidOperationException("Quiz already submitted.");
                }

                userQuiz.Schedule = null;

                await _context.UserQuizzes.AddAsync(userQuiz);
                await _context.SaveChangesAsync();

                return true.AsSuccessServiceResponse("Ok");
            }
            catch
            {
                throw;
            }
        }

        public Task<ServiceResponse<UserQuiz>> AddUserQuiz(UserQuiz userQuiz)
        {
            Guard.Against.InvalidQuizSetupPayload(userQuiz);

            try
            {
                userQuiz.Id = Guid.NewGuid();

                // format question and answers for UI
                userQuiz.Schedule.Quiz.QuizSections
                    .ForEach(section => section.Questions
                        .ForEach(question => { question.QuizAnswers = PopulateAnswers(question, userQuiz.Id, section); }
                        ));

                return Task.Run(() => userQuiz.AsSuccessServiceResponse("Ok"));
            }
            catch
            {
                throw;
            }
        }

        private async Task<List<QuizSection>> GetQuizSections(Schedule schedule)
        {
            return await _context.QuizSections
                                .Include(qs => qs.Questions.OrderBy(q => q.QuizSectionIndex))
                                .OrderBy(qs => qs.Index)
                                .Where(qs => qs.QuizId == schedule.Quiz.Id)
                                .ToListAsync();
        }

        private List<UserQuiz> GetCurrentUserCompletedNotExpiredQuizzes(UserQuizRequest userQuizRequest, User user)
        {
            return _context.UserQuizzes
                                .Where(x => x.UserId == user.Id && x.Completed.HasValue &&
                                            (x.Expiry > userQuizRequest.ClientLocalDateTime.DateTime))
                                .ToList();
        }

        private static bool CalculateWeekShift(UserQuizRequest userQuizRequest)
        {
            return userQuizRequest.ClientLocalDateTime.Day ==
                                            userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfWeek(false).Day &&
                                            userQuizRequest.ClientLocalDateTime.TimeOfDay >=
                                            TimeshiftConstants.TIME_SHIFT_START_TIME &&
                                            userQuizRequest.ClientLocalDateTime.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;
        }

        private static bool CalculateMonthShift(UserQuizRequest userQuizRequest)
        {

            // check if is time to add a new monthly test to list
            return userQuizRequest.ClientLocalDateTime.Day ==
                             userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfMonth(false).Day &&
                             userQuizRequest.ClientLocalDateTime.TimeOfDay >=
                             TimeshiftConstants.TIME_SHIFT_START_TIME &&
                             userQuizRequest.ClientLocalDateTime.TimeOfDay <=
                             TimeshiftConstants.TIME_SHIFT_END_TIME;
        }

        private async Task<List<Schedule>> GetCurrentUsersGroupSchedules(User user)
        {
            return await _context.ScheduleGroups
                                          .Include(sg => sg.Schedule)
                                            .ThenInclude(s => s.Quiz)
                                            .ThenInclude(q => q.QuizSections)
                                          .Where(sg => sg.GroupId == user.GroupId)
                                          .Select(sg => new Schedule
                                          {
                                              Id = sg.Schedule.Id,
                                              QuizId = sg.Schedule.QuizId,
                                              Quiz = new SCModels.Quiz
                                              {
                                                  Id = sg.Schedule.Quiz.Id,
                                                  Name = sg.Schedule.Quiz.Name,
                                                  Description = sg.Schedule.Quiz.Description,
                                                  InfoMarkdown = sg.Schedule.Quiz.InfoMarkdown,
                                                  Created = sg.Schedule.Quiz.Created,
                                                  Updated = sg.Schedule.Quiz.Updated
                                              },
                                              Recurrence = sg.Schedule.Recurrence,
                                              StartTime = sg.Schedule.StartTime,
                                              EndTime = sg.Schedule.EndTime,
                                              Created = sg.Schedule.Created,
                                              Updated = sg.Schedule.Updated
                                          })
                                          .ToListAsync();
        }

        private UserQuiz GenerateUserQuiz(DateTime currentDate, Guid userId, Schedule schedule, bool isShift)
        {
            var monthlyExpiry = QuizHelper.GetMonthlyExpiry(currentDate, isShift);
            var weeklyExpiry = QuizHelper.GetWeeklyExpiry(currentDate, isShift);

            return new UserQuiz
            {
                ScheduleId = schedule.Id,
                UserId = userId,
                Id = Guid.NewGuid(),
                Sequence = schedule.Recurrence == Recurrence.Monthly
                    ? monthlyExpiry.Month
                    : CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(weeklyExpiry.DateTime,
                        CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                Expiry = schedule.Recurrence == Recurrence.Monthly
                    ? new DateTime(monthlyExpiry.DateTime.Ticks, DateTimeKind.Utc)
                    : new DateTime(weeklyExpiry.DateTime.Ticks, DateTimeKind.Utc)
            };
        }

        private IList<QuizAnswer> PopulateAnswers(Question question, Guid userQuizId,
            QuizSection section)
        {
            var answers = new List<QuizAnswer>();

            var regex = new Regex("{(.*?)}");

            var matches = regex.Matches(question.Body);

            var index = 0;

            var questionBody = question.Body;

            question.Description = section.Description;

            foreach (Match match in matches)
            {
                index++;

                var prefix = questionBody.Split(new string[] { match.Value }, StringSplitOptions.None).FirstOrDefault();

                answers.Add(new QuizAnswer
                {
                    ActualAnswer = match.Groups[1].Value.Trim(),
                    QuestionId = question.Id,
                    AnswerIndex = index,
                    AnswerPrefix = prefix,
                    AnswerSuffix = index == matches.Count
                        ? questionBody.Split(new string[] { match.Value }, StringSplitOptions.None)?.LastOrDefault()
                        : string.Empty,
                    AnswerLength = match.Groups[1].Value.Length
                });


                if (!string.IsNullOrEmpty(prefix))
                    questionBody = questionBody.ReplaceStart($"{prefix}", "");

                questionBody = questionBody.ReplaceStart(match.Value, "").Trim();
            }

            return answers;
        }
    }
}
