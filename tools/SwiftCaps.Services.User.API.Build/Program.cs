namespace build
{
    partial class Program
    {
        private const string Prefix = "SwiftCaps.Services.User.API";
        
        private static readonly string[] ProjectFiles = new [] { 
            $"./Api/Function/src/SwiftCaps.Services.User.API" 
        };
        
        private static readonly string[] TestProjFiles = new [] {
            $"./Api/Function/test/SwiftCaps.Services.User.API.Tests",
            $"./Api/Service/test/SwiftCaps.Services.User.Tests"
        };
    }
}
