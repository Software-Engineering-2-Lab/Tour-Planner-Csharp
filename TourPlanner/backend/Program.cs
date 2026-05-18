using Serilog;
using Log = Serilog.Log;
using BackendNet.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TourPlanner.backend.Data;
using TourPlanner.backend.Repositories;
using TourPlanner.backend.Entities;
using TourPlanner.backend.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/tourplanner-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

try
{
    Log.Information("Starting TourPlanner Backend...");
    var app = builder.Build();

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

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCors("AllowFrontend");

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