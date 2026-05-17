using Serilog;
using BackendNet.Exceptions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/tourplanner-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// --- 1. CONFIGURARE CORS (Adăugată) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL-ul frontend-ului tău (Angular)
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

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseMiddleware<ExceptionMiddleware>();

    // --- 2. ACTIVARE CORS (Adăugată - Obligatoriu înainte de MapControllers) ---
    app.UseCors("AllowFrontend");

    // --- 3. CORECTIE DOCKER (Comentat/Scurs) ---
    // În Docker se rulează de obicei pe HTTP în interiorul containerului. 
    // Dacă frontend-ul apelează http://localhost:8080, scoate linia de mai jos:
    // app.UseHttpsRedirection(); 

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