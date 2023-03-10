using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftCAPS.Web.Shared.Extensions;
using BlazorFluentUI;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using SwiftCaps.Client.Shared.Models;
using SwiftCAPS.Web.Shared.Services;
using System.Net.Http;
using System;

namespace SwiftCAPS.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            AddGenericHttpClient(builder);

            builder.Services.AddMsalAuthentication<RemoteAuthenticationState, SwiftCapsUser>(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.LoginMode = "redirect";
                options.ProviderOptions.Cache.CacheLocation = "sessionStorage";
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://swiftcaps.api/.default");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
            }).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, SwiftCapsUser, QuizUserAccountFactory>();
            builder.Services.AddBlazorFluentUI();
            builder.Services.AddSwiftCapsServices();

            await builder.Build().RunAsync();
        }

        private static void AddGenericHttpClient(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddHttpClient("GenericHttpClient", (sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var baseAddress = new Uri(configuration["ApiGateway"]);
                var sKey = configuration["SubscriptionKey"];
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", sKey);
            });
            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("GenericHttpClient"));
        }
    }
}
