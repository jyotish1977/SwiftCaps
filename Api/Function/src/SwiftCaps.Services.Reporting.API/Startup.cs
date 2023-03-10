using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using SwiftCaps.Services.Reporting.Extensions;
using Xamariners.Functions.Core.Extensions;

namespace SwiftCaps.Services.Reporting.Api
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            AddConfiguration(services);
            services.AddFunctionsCoreServices();
            services.AddReportingServices();
            return services;
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? "development";
            if (!env.Equals("development", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
            services.AddSingleton(config);
        }
    }
}
