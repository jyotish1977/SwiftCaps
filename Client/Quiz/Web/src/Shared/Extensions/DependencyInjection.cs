using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SwiftCaps.Client.Shared.Services.Abstraction;
using SwiftCaps.Client.Shared.Services.Implementation;
using SwiftCAPS.Web.Shared.Clients;
using SwiftCAPS.Web.Shared.State;

namespace SwiftCAPS.Web.Shared.Extensions
{
    public static class DependencyInjection
    {
        private static RefitSettings _refitSettings => new RefitSettings(
            new NewtonsoftJsonContentSerializer());
    
        public static void AddSwiftCapsServices(this IServiceCollection services)
        {   
            services.AddScoped<IUserApplicationService, UserApplicationService>();
            services.AddScoped<UserState>();
            AddUserClient(services);
            AddQuizClient(services);
            AddReportingClient(services);
        }

        private static void AddQuizClient(IServiceCollection services)
        {                                    
            ConfigureClient(services.AddRefitClient<IQuizClient>(_refitSettings));
        }

        private static void AddUserClient(IServiceCollection services)
        {
            ConfigureClient(services.AddRefitClient<IUserClient>(_refitSettings));
        }

        private static void AddReportingClient(IServiceCollection services)
        {
            ConfigureClient(services.AddRefitClient<IReportingClient>(_refitSettings));
        }

        private static void ConfigureClient(IHttpClientBuilder clientBuilder)
        {
            clientBuilder.ConfigureHttpClient((sp, c) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    c.BaseAddress = new Uri(configuration["ApiGateway"]);
                    c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["SubscriptionKey"]);
                })
                .AddHttpMessageHandler(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var apiUri = new Uri(configuration["ApiGateway"]);
                    return sp.GetRequiredService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] {apiUri.GetLeftPart(UriPartial.Authority)},
                            scopes: new[] {"https://swiftcaps.api/.default"});
                });

            // TODO MAYBE DO - https://docs.microsoft.com/en-us/aspnet/core/migration/31-to-50?view=aspnetcore-5.0&tabs=visual-studio
            //builder.Services.AddHttpClient("{APP NAMESPACE}.ServerAPI", 
            //client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            //AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
            //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            //.CreateClient("{APP NAMESPACE}.ServerAPI"));

        }

      
    }
}
