using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    public class LeaderBoardCacheService
    {
        private readonly ISwiftCapsApiServices _services;
        public IDateTimeOffsetProvider DateTimeOffsetProvider { get; }


        public LeaderBoardCacheService(ISwiftCapsApiServices services, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _services = services;
            DateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        internal async Task<ServiceResponse<bool>> ClearLeaderBoardCache(string appDataPath)
        {
            // brute clear data
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                if (!dbContext.LeaderBoards.Any())
                    return new ServiceResponse<bool>(ServiceStatus.Success);

                try
                {
                    dbContext.QuizReports.RemoveRange(dbContext.QuizReports.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    dbContext.LeaderBoards.RemoveRange(dbContext.LeaderBoards.ToList());
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>(false, "Error Clearing LeaderBoards Cache", ex.Message, ex);
                }
            }

            return new ServiceResponse<bool>(ServiceStatus.Success);
        }

        public async Task<ServiceResponse<bool>> SetLeaderBoardCache(string appDataPath, Guid memberId)
        {
            var response = await _services.LeaderBoardService.GetLeaderBoard(
                new UserQuizRequest
                {
                    UserId = memberId, ClientLocalDateTime = DateTimeOffsetProvider.Now
                }).ConfigureAwait(false);

            if (!response.IsOK())
                return response.ToServiceResponse<IList<LeaderBoard>, bool>();

            // brute clear data and repopulate
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                try
                {
                    await ClearLeaderBoardCache(appDataPath).ConfigureAwait(false);
                    await dbContext.LeaderBoards.AddRangeAsync(response.Data).ConfigureAwait(false);
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>(false, "Error Setting LeaderBoards Cache", ex.Message, ex);
                }
            }

            return new ServiceResponse<bool>(ServiceStatus.Success);
        }

        public async Task<ServiceResponse<List<LeaderBoard>>> GetLeaderBoardCache(string appDataPath)
        {
            using (var dbContext = new SwiftCapsClientContext(appDataPath))
            {
                var result = await dbContext.LeaderBoards
                    .Include(x => x.QuizReports)
                    .Include(x => x.MonthlyQuizReports)
                    .Include(x => x.WeeklyQuizReports)
                    .OrderBy(x => x.UserName)
                    .ToListAsync()
                    .ConfigureAwait(false);
                return result.AsServiceResponse();
            }
        }
    }
}