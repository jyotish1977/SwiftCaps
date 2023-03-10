using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftCaps.Data.Context;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Admin.Services.Quiz.Extensions
{
    public static class DependencyInjection
    {
        public static void AddAdminQuizServices(this IServiceCollection services)
        {
            services.AddDbContext<SwiftCapsContext>((sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration["ScDbConnection"]);
            });
            services.AddScoped<IAdminQuizService, AdminQuizService>();
            services.AddScoped<IAdminQuizSectionService, AdminQuizSectionService>();
            services.AddScoped<IAdminQuizQuestionService, AdminQuizQuestionService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IScheduleGroupService, ScheduleGroupService>();
            services.AddScoped<IAdminReportingService, AdminReportingService>();
            services.AddScoped<IAdminGroupService, AdminGroupService>();
        }
    }
}
