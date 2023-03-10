using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftCAPS.Admin.Web.Shared.Extensions;
using BlazorFluentUI;

namespace SwiftCAPS.Admin.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.LoginMode = "redirect";
                options.ProviderOptions.Cache.CacheLocation = "sessionStorage";
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://swiftcaps.api/.default");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
            });

            builder.Services.AddBlazorFluentUI();
            builder.Services.AddSwiftCapsAdminServices(builder.Configuration);

            await builder.Build().RunAsync();
        }
    }
}
