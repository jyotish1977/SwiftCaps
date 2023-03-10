using System;

namespace build
{
    partial class Program
    {
        private const string Prefix = "SwiftCaps.Services.Reporting.API";

        private static readonly string[] ProjectFiles = new [] {
            $"./Api/Function/src/SwiftCaps.Services.Reporting.API"
        };

        private static readonly string[] TestProjFiles = new [] {
            "./Api/Function/test/SwiftCaps.Services.Reporting.API.Tests",
            "./Api/Service/test/SwiftCaps.Services.Reporting.Tests"
        };
    }
}
