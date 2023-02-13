using CROP.API.Data;
using CROP.API.Services;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using static CROP.API.Services.RedisService;

namespace CROP.API
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerJwtSupport();

            builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration[Env.RedisConnection] ?? builder.Configuration["Redis:Connection"] ?? "redis://localhost"));
            builder.Services.AddHostedService<IndexCreationService>();
            builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(builder.Configuration[Env.PostgresConnection] ?? builder.Configuration["PostgreSQL:Connection"]));

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<DbInitializer>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.InitializeDatabase();

            app.Run();
        }
    }
}

