using System;
using System.Threading;
using System.Threading.Tasks;
using SwiftCaps.Client.Cache.Service.Services;
using Xamariners.RestClient.Helpers.Models;

namespace SwiftCaps.Client.Cache.Service.Interfaces
{
    public interface ISwiftCapsCacheServices
    {
        // main services
        QuizCacheService QuizCacheService { get; }
        LeaderBoardCacheService LeaderBoardCacheService { get;  }

        Task<ServiceResponse<bool>> Refresh(string appDataPath, Guid userId);
        
        Task<ServiceResponse<bool>> DeleteDatabase(string appDataPath);

        ManualResetEvent WaitHandle { get; }
    }
} 
