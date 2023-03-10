using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Data;
using SwiftCaps.Client.Cache.Service.Interfaces;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Infrastructure;
using Xamarin.Essentials;
using Xamariners.RestClient.Helpers;
using Xamariners.RestClient.Helpers.Extensions;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Client.Cache.Service.Services
{
    public class SwiftCapsCacheServices : ISwiftCapsCacheServices
    {
        // locks the refresh process thread
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // control time
        private static ManualResetEvent _waitHandle = new ManualResetEvent(false);

    
        public ManualResetEvent WaitHandle => _waitHandle;

        public SwiftCapsCacheServices(QuizCacheService quizCacheService, LeaderBoardCacheService leaderBoardCacheService)
        {
            QuizCacheService = quizCacheService;
            LeaderBoardCacheService = leaderBoardCacheService;
        }

        public QuizCacheService QuizCacheService { get; }
        public LeaderBoardCacheService LeaderBoardCacheService { get;  }

        public async Task<ServiceResponse<bool>> Refresh(string appDataPath, Guid userId)
        { 
            _semaphore.Wait();
            WaitHandle.Reset();

            try
            {
                // CLEAR    
                var deleteResult = await DeleteDatabase(appDataPath)
                    .ConfigureAwait(false);
                if (!deleteResult.IsOK())
                        return deleteResult;
                    
                // SET
                var quizResult = await QuizCacheService.SetUserQuizCache(appDataPath, userId)
                    .ConfigureAwait(false);
                if (!quizResult.IsOK())
                    return quizResult;

                var leaderBoardResult = await LeaderBoardCacheService.SetLeaderBoardCache(appDataPath, userId)
                    .ConfigureAwait(false);
                if (!leaderBoardResult.IsOK())
                    return leaderBoardResult;

                return true.AsServiceResponse();
            }
            finally
            {   
                _semaphore.Release();
                WaitHandle.Set();
            }
        }
        public async Task<ServiceResponse<bool>> DeleteDatabase(string appDataPath)
        {
            bool result;
           
            //// brute clear data and repopulate
            using (var dbContext = new SwiftCapsClientContext(appDataPath, false))
            {
                result = await RetryHelpers.Retry(async () =>
                        await dbContext.Database.EnsureDeletedAsync(CancellationToken.None).ConfigureAwait(false), 500)
                    .ConfigureAwait(false);
            }

            return result.AsServiceResponse();
        }
    }
}
