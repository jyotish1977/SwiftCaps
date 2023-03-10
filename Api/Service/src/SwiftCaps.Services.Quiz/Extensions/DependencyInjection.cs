﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftCaps.Data.Context;
using SwiftCaps.Services.Abstraction.Interfaces;

namespace SwiftCaps.Services.Quiz.Extensions
{
    public static class DependencyInjection
    {
        public static void AddQuizServices(this IServiceCollection services)
        {
            services.AddDbContext<SwiftCapsContext>((sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration["ScDbConnection"]);
            });
            services.AddScoped<IQuizService, QuizService>();
        }
    }
}
