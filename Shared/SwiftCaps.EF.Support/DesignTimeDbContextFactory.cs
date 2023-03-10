using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SwiftCaps.Data.Context;

namespace SwiftCaps.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SwiftCapsContext>
    {
        public SwiftCapsContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SwiftCapsContext>();
          
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var conn = config["SqlConnectionString"];
            builder.UseSqlServer(conn);
            return new SwiftCapsContext(builder.Options);
        }
    }
}