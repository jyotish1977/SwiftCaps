using System;

namespace build
{
    partial class Program
    {
        private const string Prefix = "SwiftCaps.Services.Quiz.API";

        private static readonly string[] ProjectFiles = new[] {
            "./Api/Function/src/SwiftCaps.Services.Quiz.API"
        };

        private static readonly string[] TestProjFiles = new[] {
            "./Api/Function/test/SwiftCaps.Services.Quiz.API.Tests",
            "./Api/Service/test/SwiftCaps.Services.Quiz.Tests"
        };
    }
}
