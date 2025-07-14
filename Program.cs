using BookStoreApi;
using BookStoreApi.Configurations;
using BookStoreApi.Data;
using BookStoreApi.Extensions;
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using BookStoreApi.Services.Email;
using BookStoreApi.Services.FileStorage;
using BookStoreApi.Sheduled_Tasks;
using Microsoft.Extensions.FileProviders;
using NLog;
using NLog.Web;


var logger = NLog.LogManager.Setup().GetCurrentClassLogger();

try
{
    logger.Debug("Application starting up and NLog initialized for early logging.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    logger.Info("ASP.NET Core host builder setup completed. NLog configuration loaded from appsettings.json.");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddJsonSerializationOptions(); 
    builder.Services.AddApplicationServices();       
    builder.Services.AddApplicationDbContext(builder.Configuration); 
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddIdentityConfiguration();    
    builder.Services.AddJwtAuthentication(builder.Configuration); 
    builder.Services.AddGlobalExceptionHandler();
    builder.Services.AddControllers();
    builder.Services.AddAppFluentValidation();
    builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
    builder.Services.AddScoped<IWishlistService, WishlistService>();
    builder.Services.AddMemoryCache();

    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

    builder.Services.AddScoped<INewArrivalsProcessor, NewArrivalsProcessor>();
    builder.Services.AddHostedService<NewArrivalsNewsletterScheduler>();

    builder.Services.AddHostedService<RentalReminderService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();

    builder.Services.AddScoped<IEmailService,SmtpEmailService>();

    builder.Services.AddScoped<IRentalService,RentalService>();

    builder.Services.Configure<BackgroundServiceSettings>(
    builder.Configuration.GetSection("BackgroundServices"));


    var app = builder.Build();

   // await app.SeedRolesAndAdminUserAsync();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            await DbInitializer.SeedRoles(services);
        }
        catch (Exception ex)
        {
            var log = services.GetRequiredService<ILogger<Program>>();
            log.LogError(ex, "An error occurred while seeding the database roles.");
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler();
    }

    string uploadsFolder = app.Configuration["FileStorageSettings:UploadsFolder"] ?? "Uploads";
    string uploadsPath = Path.Combine(app.Environment.ContentRootPath, uploadsFolder);

    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsPath),
        RequestPath = $"/{uploadsFolder}" 
    });


    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of an unhandled exception.");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}