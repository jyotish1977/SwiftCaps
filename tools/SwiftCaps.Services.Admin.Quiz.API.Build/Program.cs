using System;

namespace build
{
    partial class Program
    {
        private const string Prefix = "SwiftCaps.Services.Admin.Quiz.API";

        private static readonly string[] ProjectFiles = new[] {
            $"./Api/Function/src/SwiftCaps.Admin.Services.Quiz.API"
        };

        private static readonly string[] TestProjFiles = new [] {
            "./Api/Function/test/SwiftCaps.Admin.Services.Quiz.API.Tests",
            "./Api/Service/test/SwiftCaps.Admin.Services.Quiz.Tests"
        };
    }
}
