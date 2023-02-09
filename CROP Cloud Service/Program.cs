using CROP.API.Data;
using CROP.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Redis.OM;
using System.Text;
using static CROP.API.Services.RedisService;

namespace CROP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.UseUrls(new string[] { builder.Configuration["Url:Http"] ?? "", builder.Configuration["Url:Https"] ?? "" });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerJwtSupport();

            builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["Redis:Connection"] ?? "redis://localhost"));
            builder.Services.AddHostedService<IndexCreationService>();
            builder.Services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(builder.Configuration["PostgreSQL:Connection"]));

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

