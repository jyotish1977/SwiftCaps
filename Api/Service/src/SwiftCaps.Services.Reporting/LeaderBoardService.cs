using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using SwiftCaps.Data.Context;
using SwiftCaps.Models.Constants;
using SwiftCaps.Models.Enums;
using SwiftCaps.Models.Models;
using SwiftCaps.Models.Requests;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.Reporting.Extensions;
using Xamariners.Core.Common.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Services.Reporting
{
    public class LeaderBoardService : ILeaderBoardService
    {
        private readonly SwiftCapsContext _context;

        public LeaderBoardService(SwiftCapsContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IList<LeaderBoard>>> GetLeaderBoard(UserQuizRequest userQuizRequest)
        {
            Guard.Against.InvalidLeaderBoardReadPayload(userQuizRequest);
            
            // get current user
            var thisUser = await _context.Users.FindAsync(userQuizRequest.UserId);
            if(thisUser == null)
            {
                throw new NotFoundException(userQuizRequest.UserId.ToString(),"User");
            }

            // get users from same group
            var usersList = _context.Users.Where((x => x.GroupId == thisUser.GroupId)).ToList();


            // check if is time to add a new monthly test to list
            var monthShift = userQuizRequest.ClientLocalDateTime.Day == userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfMonth(false).Day &&
                             userQuizRequest.ClientLocalDateTime.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME && userQuizRequest.ClientLocalDateTime.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;

            // check if is time to add a new weekly test to list
            var weekShift = userQuizRequest.ClientLocalDateTime.Day == userQuizRequest.ClientLocalDateTime.DateTime.LastDayOfWeek(false).Day &&
                            userQuizRequest.ClientLocalDateTime.TimeOfDay >= TimeshiftConstants.TIME_SHIFT_START_TIME && userQuizRequest.ClientLocalDateTime.TimeOfDay <= TimeshiftConstants.TIME_SHIFT_END_TIME;

            var leaderBoards = await GenerateLeaderBoards(usersList, userQuizRequest.ClientLocalDateTime, weekShift, monthShift);
            
            return leaderBoards.AsSuccessServiceResponse<IList<LeaderBoard>>("Ok");
        }

        private async Task<List<LeaderBoard>> GenerateLeaderBoards(IList<User> users, DateTimeOffset clientLocalDateTime, bool weekShift, bool monthShift)
        {
            var leaderBoards = new List<LeaderBoard>();
            // for each user from user group
            foreach (var user in users)
            {
                //create object to add to returnable list
                var leaderBoardRow = new LeaderBoard
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    MonthlyQuizReports = new List<QuizReport>(),
                    WeeklyQuizReports = new List<QuizReport>()
                };

                var userQuizzes = _context.UserQuizzes
                    .Where(x => x.UserId == user.Id &&
                                (x.Expiry > clientLocalDateTime.DateTime))
                    ?.ToList();

                // no user quiz found, add empty user quiz to report
                if (userQuizzes?.Count == 0)
                {
                    leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                    leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());

                    if (weekShift)
                    {
                        leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                    }
                    if (monthShift)
                    {
                        leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                    }
                }
                else
                {
                    foreach (var userQuiz in userQuizzes)
                    {
                        //Schedule of that user's each group quiz
                        var schedule = await _context.Schedules.FindAsync(userQuiz.ScheduleId);
                        userQuiz.Schedule = schedule;

                        if (userQuiz.Schedule.Recurrence == Recurrence.Weekly)
                        {
                            leaderBoardRow.WeeklyQuizReports.Add(new QuizReport
                            {
                                Recurrence = userQuiz.Schedule.Recurrence,
                                IsCompleted = userQuiz.Completed.HasValue,
                                QuizId = userQuiz.Schedule.QuizId,
                                ExpiryDate = userQuiz.Expiry
                            });
                            //Because timeshift if only 1 quiz add 1 more empty quiz report
                            if (weekShift && userQuizzes.Count(x => x.Schedule?.Recurrence == Recurrence.Weekly) == 1)
                            {
                                leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                            }
                        }
                        else
                        {
                            leaderBoardRow.MonthlyQuizReports.Add(new QuizReport
                            {
                                Recurrence = userQuiz.Schedule.Recurrence,
                                IsCompleted = userQuiz.Completed.HasValue,
                                QuizId = userQuiz.Schedule.QuizId,
                                ExpiryDate = userQuiz.Expiry
                            });
                            //Because timeshift if only 1 quiz add 1 more empty quiz report
                            if (monthShift && userQuizzes.Count(x => x.Schedule?.Recurrence == Recurrence.Monthly) == 1)
                            {
                                leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                            }
                        }
                    }

                    if (!leaderBoardRow.WeeklyQuizReports.Any())
                    {
                        // add an empty weekly report for the row
                        leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());

                        if (weekShift)
                        {
                            leaderBoardRow.WeeklyQuizReports.Add(new QuizReport());
                        }
                    }

                    if (!leaderBoardRow.MonthlyQuizReports.Any())
                    {
                        // add an empty weekly report for the row
                        leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());

                        if (monthShift)
                        {
                            leaderBoardRow.MonthlyQuizReports.Add(new QuizReport());
                        }
                    }
                }

                leaderBoards.Add(leaderBoardRow);
            }
            return leaderBoards;
        }
    }
}

