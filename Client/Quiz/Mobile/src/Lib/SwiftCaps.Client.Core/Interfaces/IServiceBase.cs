using SwiftCaps.Client.Core.Services;
using SwiftCaps.Client.Core.Services.Infrastructure;
using Xamariners.RestClient.Interfaces;

namespace SwiftCaps.Client.Core.Interfaces
{
    public interface IServiceBase
    {
        IAppSettings AppSettings { get; set; }
        IRestClient RestClient { get; set; }
        IAppCacheService<ClientState> AppCache { get; set; }
    }
}