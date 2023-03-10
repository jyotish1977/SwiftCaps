using System.IO;
using Microsoft.Extensions.Configuration;

namespace SwiftCaps.Data.Seeder
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build(); new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            Seeder.Seed(config);
        }
    }
}
