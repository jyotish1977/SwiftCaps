﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xamariners.Core.Interface;
using Xamariners.Functions.Core.Configuration;
using Xamariners.Functions.Core.Interfaces;
using Xamariners.Functions.Core.Services;

namespace Xamariners.Functions.Core.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFunctionsCoreServices(this IServiceCollection services)
        {
            AddFunctionsCoreConfiguration(services);
            services.AddScoped<IFunctionAuthorizationService, FunctionAuthorizationService>();
            services.AddSingleton<ILogger, RaygunLogger>();
            return services;
        }

        private static void AddFunctionsCoreConfiguration(IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var apiConfig = new AzureADConfiguration();
                var configuration = sp.GetService<IConfiguration>();
                configuration.Bind(apiConfig);
                return apiConfig;
            });
            services.AddSingleton(sp =>
            {
                var apiConfig = new RaygunConfiguration();
                var configuration = sp.GetService<IConfiguration>();
                configuration.Bind(apiConfig);
                return apiConfig;
            });
        }
    }
}
