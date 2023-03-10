using SwiftCaps.Client.Core.Services.Infrastructure;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Services.Abstraction;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Client.Core.Interfaces
{
    public interface ISwiftCapsApiServices
    {
        // main services
        IQuizService QuizService { get; }
        IUserService UserService { get; }
        IAuthService AuthService { get;  }
        ILeaderBoardService LeaderBoardService { get;  }

        // This is to expose dependency from base to the implementation. Do not remove!
        // And do not implement the properties in ApiServices.cs class.
        IAppSettings AppSettings { get; set; }
        IAppCacheService<ClientState> AppCache { get; set; }
      
    }
} 
