using BookStoreApi.Data;
using BookStoreApi.Infrastructure;
//using BookStoreApi.Middlewares;
using BookStoreApi.Models; // Ensure this is where your Author/Publisher/Book models are.
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Keep if you use JWT Bearer authentication
using Microsoft.AspNetCore.Identity; // Keep if you use Identity for user management
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web; // NLog specific using statement

// --- NLog: Setup NLog for ASP.NET Core (BEGIN) ---
// This line initializes NLog's configuration from nlog.config
var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    // You can add an initial log message here to confirm NLog is working
    logger.Debug("Application starting up...");

    var builder = WebApplication.CreateBuilder(args);

    // Clear existing logging providers and add NLog
    builder.Logging.ClearProviders(); // Removes default console, debug, etc. loggers
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); // Set the minimum log level for .NET Core's logger
    builder.Host.UseNLog(); // Integrates NLog with the ASP.NET Core hosting environment
    // --- NLog: Setup NLog for ASP.NET Core (END) ---


    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Repository and Service Registrations (Keep these as they are)
    // The commented out line below was causing an issue previously, it should remain commented or removed.
    // builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
    builder.Services.AddScoped<IPublisherService, PublisherService>();

    var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IBookService, BookService>();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else 
    {
        app.UseExceptionHandler();
      //  app.UseCustomExceptionHandling();
    }
    

    // Keep UseAuthentication/UseAuthorization if you have them for Identity (not in provided snippet, but common)
    // app.UseAuthentication();
    // app.UseAuthorization();

    app.MapControllers(); // Ensures your controllers are mapped to routes

    app.Run();
}
catch (Exception exception)
{
    // NLog: Catch setup or runtime errors that prevent the app from starting cleanly
    logger.Error(exception, "Stopped program because of an unhandled exception.");
    throw; // Re-throw to indicate failure
}
finally
{
    // NLog: Ensure all pending log messages are flushed and internal timers/threads are stopped
    NLog.LogManager.Shutdown();
}