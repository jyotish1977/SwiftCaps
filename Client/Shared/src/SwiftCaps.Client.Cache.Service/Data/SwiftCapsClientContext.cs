using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Client.Cache.Service.Data
{
    public class SwiftCapsClientContext : DbContext
    {
        private readonly string _appDataPath;

        internal const string DbName = "swiftcaps.db3";

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<UserQuiz> UserQuizzes { get; set; }

        public DbSet<QuizSection> QuizSections { get; set; }

        public DbSet<LeaderBoard> LeaderBoards { get; set; }

        public DbSet<QuizReport> QuizReports { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleGroup> ScheduleGroups { get; set; }
        public SwiftCapsClientContext(string appDataPath, bool create = true)
        {
            if (string.IsNullOrEmpty(appDataPath))
                throw new Exception("AppData path not set");

            _appDataPath = appDataPath;

            if (create)
                Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(_appDataPath, DbName);

            // NOTE: We are ignoring the FK constraints
            optionsBuilder
                .UseSqlite($"Filename={dbPath};Foreign Keys = False")
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .EnableServiceProviderCaching();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PK
            modelBuilder
                .Entity<Group>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);
            
            modelBuilder
                .Entity<Question>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Quiz>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);
            
            modelBuilder
                .Entity<QuizSection>()
                .HasKey(x => x.Id);
            
            modelBuilder
                .Entity<UserQuiz>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<LeaderBoard>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<QuizReport>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<User>()
                .Ignore(x => x.CreatedByRole)
                .Ignore(x => x.Media)
                .Ignore(x => x.DeviceTokens)
                .Ignore(x => x.DeviceTokensData)
                .HasKey(x => x.Id);


            modelBuilder
                .Entity<Schedule>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

             modelBuilder
                .Entity<ScheduleGroup>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
