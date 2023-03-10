using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Data.Context
{
    public class SwiftCapsContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<UserQuiz> UserQuizzes { get; set; }

        public DbSet<QuizSection> QuizSections { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleGroup> ScheduleGroups { get; set; }

        private static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[] {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public SwiftCapsContext(DbContextOptions<SwiftCapsContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
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
                .Entity<User>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<UserQuiz>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<UserQuiz>()
                .HasOne(uq => uq.Schedule)
                .WithMany(s => s.UserQuizzes)
                .HasForeignKey(uq => uq.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder
                .Entity<Schedule>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

             modelBuilder
                .Entity<Schedule>()
                .HasMany(s => s.ScheduleGroups)
                .WithOne(sg => sg.Schedule)
                .HasForeignKey(sg => sg.ScheduleId);

            modelBuilder
                .Entity<ScheduleGroup>()
                .Ignore(x => x.CreatedByRole)
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<ScheduleGroup>()
                .HasIndex(x => new  { x.ScheduleId, x.GroupId }).IsUnique();

             modelBuilder
                .Entity<ScheduleGroup>()
                .HasOne(sg => sg.Schedule)
                .WithMany(s => s.ScheduleGroups)
                .HasForeignKey(sg => sg.ScheduleId);

            // IGNORE

            modelBuilder
                .Entity<Question>()
                .Ignore(x => x.QuizAnswers)
                .Ignore(x => x.Description)
                .Ignore(x => x.HasDescription)
                .Ignore(x => x.HasHeader)
                .Ignore(x => x.HasFooter);

            modelBuilder
                .Entity<QuizSection>()
                .Ignore(x => x.IsValid);

            modelBuilder
                .Entity<User>()
                .Ignore(x => x.CreatedByRole)
                .Ignore(x => x.DeviceTokens)
                .Ignore(x => x.DeviceTokensData)
                .Ignore(x => x.Media)
                .Ignore(x => x.NewPassword)
                .Ignore(x => x.Password)
                .Ignore(x => x.UserRole);

            base.OnModelCreating(modelBuilder);
        }
    }
}
