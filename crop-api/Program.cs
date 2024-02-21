using CROP.API.Data;
using CROP.API.Services;
using CROP.API.Utility;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using static CROP.API.Services.RedisService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Configuration[Env.CORS]?.ToUpper() == "TRUE")
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });
}

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

app.UseAuthentication();
app.UseAuthorization();

if (builder.Configuration[Env.CORS]?.ToUpper() == "TRUE")
{
    app.UseCors();
}

app.MapControllers();

app.InitializeDatabase();

app.Run();
