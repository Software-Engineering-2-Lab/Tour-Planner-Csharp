using Serilog;
using Log = Serilog.Log;
using BackendNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TourPlanner.backend.Data;
using TourPlanner.backend.Repositories;
using TourPlanner.backend.Entities;
using TourPlanner.backend.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/tourplanner-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// External Services HTTP Clients
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();

// Database Context Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Dependency Injection Registrations
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ExtraMegaSuperSecureSecurityTokenKey"))
    };
});

// Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

try
{
    Log.Information("Starting TourPlanner Backend...");
    var app = builder.Build();

    // Automatic Database Migrations on Start-up
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Log.Information("Applying pending database migrations...");
        db.Database.Migrate();
        Log.Information("Database is up to date.");
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // Middleware Pipeline (Ordinea contează!)
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCors("AllowFrontend");
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}