using System;
using System.Linq;
using CommonServiceLocator;
using IdentityModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SwiftCaps.Client.Bootstrap;
using SwiftCaps.Data.Context;
using SwiftCaps.Fake.Data;
using Unity;
using Unity.Lifetime;

namespace SwiftCaps.Fake.Infrastructure
{
    public class FakeDbContext
    {
        public static string SwiftCapsContextConnectionName;
        public static SwiftCapsContext Context { get; set; }
        public static SqliteConnection Connection { get; set; }

        public static void SeedSwiftCapsDb()
        {  
            try
            {
                Context.Groups.AddRange(FakeGroupData.Data);
                Context.Users.AddRange(FakeUserData.Data);
                Context.SaveChanges();

                Context.Quizzes.AddRange(FakeQuizData.Data);
                Context.SaveChanges();

                Context.Schedules.AddRange(FakeQuizScheduleData.Data);
                Context.ScheduleGroups.AddRange(FakeQuizScheduleGroupData.Data);
                Context.SaveChanges();

                Context.UserQuizzes.AddRange(FakeUserQuizData.Data);
                Context.SaveChanges();

                if (!Context.QuizSections.Any())
                {
                    Context.QuizSections.AddRange(FakeQuizSectionData.Data);
                    try
                    {
                        Context.SaveChanges();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (!Context.Questions.Any())   
                {
                    Context.Questions.AddRange(FakeQuestionData.Data);
                    Context.SaveChanges();
                }
            }
            catch
            {
                // ignored
            }
        }

        public static string InitDbContext<T>() where T : DbContext
        {
            Connection = new SqliteConnection("DataSource=:memory:");

            Connection.Open();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .AddDbContext<T>(
                    o => o
                        .UseSqlite(Connection)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                        .UseLoggerFactory(new NullLoggerFactory()),
                    ServiceLifetime.Transient)
                .BuildServiceProvider();

            Context = serviceProvider.GetService<SwiftCapsContext>();

            Context.Database.EnsureCreated();

            BootStrapper.Container.RegisterInstance(Context);
            var name = Context.GetType().Name + "Connection)";

            // POPULATE DATABASE
            BootStrapper.Container.RegisterInstance(name, Connection);
            BootStrapper.Container.RegisterInstance(new DbContextOptions<T>());

            return name;
        }

        public static void Terminate()
        {
            DetachAllEntities(Context);

            Context.Database.EnsureDeleted();

            Connection.Close();
            Connection.Dispose();
            Connection = null;

            Context.Dispose();
            Context = null;
        }

        public static void DetachAllEntities(SwiftCapsContext context)
        {
            try
            {
                var changedEntriesCopy = context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added ||
                                e.State == EntityState.Modified ||
                                e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entry in changedEntriesCopy)
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch
            {}
        }
    }
}
