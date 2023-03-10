using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Client.Core.Services.Interfaces;
using SwiftCaps.Services.Abstraction;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Client.Core.Services.Infrastructure
{
    public class SwiftCapsApiServices : ServiceBase, ISwiftCapsApiServices
    {   
        public SwiftCapsApiServices(IQuizService quizService, IUserService userService, IAuthService authService, ILeaderBoardService leaderBoardService)
        {
            UserService = userService;
            AuthService = authService;
            QuizService = quizService;
            LeaderBoardService = leaderBoardService;
           
            // ... etc.
        }

        public IQuizService QuizService { get; }
        public IUserService UserService { get; }
        public IAuthService AuthService { get;  }
        public ILeaderBoardService LeaderBoardService { get;  }

        // ... etc.

    }
}
