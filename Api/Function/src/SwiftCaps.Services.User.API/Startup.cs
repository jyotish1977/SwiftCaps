using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftCaps.Services.User.Extensions;
using Xamariners.Functions.Core.Extensions;

namespace SwiftCaps.Services.User.API
{
    public static class Startup 
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            AddConfiguration(services);
            services.AddFunctionsCoreServices();
            services.AddUserServices();
            return services;
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? "development";
            Console.WriteLine($"AddConfiguration: env - $(env)");
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
