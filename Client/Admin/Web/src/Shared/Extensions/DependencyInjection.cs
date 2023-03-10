using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SwiftCaps.Client.Shared.Services.Abstraction;
using SwiftCaps.Client.Shared.Services.Implementation;
using SwiftCAPS.Admin.Web.Shared.Clients;
using SwiftCAPS.Admin.Web.Shared.States;

namespace SwiftCAPS.Admin.Web.Shared.Extensions
{
    public static class DependencyInjection
    {
        private static RefitSettings _refitSettings => new RefitSettings(
            new NewtonsoftJsonContentSerializer());

        public static void AddSwiftCapsAdminServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserApplicationService, UserApplicationService>();
            services.AddScoped<UserState>();
            AddAdminQuizClient(services);
        }

        private static void AddAdminQuizClient(IServiceCollection services)
        {
            ConfigureClient(services.AddRefitClient<IAdminQuizClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IAdminQuizSectionClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IQuestionClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IScheduleClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IScheduleGroupClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IReportingClient>(_refitSettings));
            ConfigureClient(services.AddRefitClient<IGroupClient>(_refitSettings));
        }

        private static void ConfigureClient(IHttpClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureHttpClient((sp, c) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                c.BaseAddress = new Uri(configuration["AdminApiGateway"]);
                c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["SubscriptionKey"]);
            })
                .AddHttpMessageHandler(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var apiUri = new Uri(configuration["AdminApiGateway"]);
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { apiUri.GetLeftPart(UriPartial.Authority) },
                            scopes: new[] { "https://swiftcaps.api/.default" });
                });
        }
    }
}
