using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using SwiftCaps.Data.Context;
using SwiftCaps.Services.Abstraction.Interfaces;
using SwiftCaps.Services.User.Clients;

namespace SwiftCaps.Services.User.Extensions
{
    public static class DependencyInjection
    {
        public static void AddUserServices(this IServiceCollection services)
        {
            const string aadAuthUrl = "https://login.windows.net/{0}/oauth2/v2.0";
            const string graphUrl = "https://graph.microsoft.com";

            services.AddRefitClient<IAuthenticationClient>()
                .ConfigureHttpClient((sp, c) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var url = string.Format(aadAuthUrl, config["TenantId"]);
                    c.BaseAddress = new Uri(url);
                });

            services.AddRefitClient<IGraphClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(graphUrl));


            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();

            services.AddDbContext<SwiftCapsContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                options.UseSqlServer(config["ScDbConnection"]);
            });
        }
    }
}
