using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Client.Core.Interfaces;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Data;
using SwiftCaps.Services.Abstraction;
using SwiftCaps.Services.Abstraction.Interfaces;
using Unity;
using Xamariners.RestClient.Interfaces;

namespace SwiftCaps.Fake.Infrastructure
{
    public static class FakeContainerInitialiser
    {

        public static void RegisterFakeService()
        {
            BootStrapper.Container.AddExtension(new Diagnostic());

            // Proper Server Services
            BootStrapper.Container.RegisterType<IQuizService, SwiftCaps.Services.Quiz.QuizService>();
            BootStrapper.Container.RegisterType<IUserService, SwiftCaps.Services.User.UserService>();
            BootStrapper.Container.RegisterType<IGroupService, SwiftCaps.Services.User.GroupService>();
            BootStrapper.Container.RegisterType<ILeaderBoardService, SwiftCaps.Services.Reporting.LeaderBoardService>();

            // Pure Fake Stuff
            BootStrapper.Container.RegisterType<IAdAuthService, SwiftCaps.Fake.Services.AdAuthService>();
            BootStrapper.Container.RegisterType<IGraphService, SwiftCaps.Fake.Services.GraphService>();
        }

        public static void SetupFakeDatabase()
        {
            FakeDbContext.SwiftCapsContextConnectionName = FakeDbContext.InitDbContext<SwiftCapsContext>();
        }

        public static void SeedFakeDatabase()
        {
            //ResetFakeData();
            FakeDbContext.SeedSwiftCapsDb();
        }

        public static void ResetFakeData()
        {
            FakeGroupData.Init();
            FakeUserData.Init();
            FakeQuizData.Init();
            FakeQuizSectionData.Init();
            FakeQuestionData.Init();
            FakeQuizAnswerData.Init();
            FakeQuizScheduleData.Init();
            FakeQuizScheduleGroupData.Init();
            FakeUserQuizData.Init();
        }
    }
}
