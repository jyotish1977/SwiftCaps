using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SwiftCaps.Services.Reporting.Api
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((hostContext, builder) => {
                    var environment = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? "Development";
                    if (environment == "Development")
                    {
                        builder.AddUserSecrets<Program>();
                    }
                })
                .ConfigureServices(services => Startup.ConfigureServices(services))
                .Build();

            await host.RunAsync();
        }
    }
}
