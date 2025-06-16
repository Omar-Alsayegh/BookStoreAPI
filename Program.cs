using BookStoreApi.Data;
using BookStoreApi.Infrastructure;
using BookStoreApi.Models;
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using BookStoreApi.Services.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Linq.Dynamic.Core; // For generic sorting
using System.Text.Json.Serialization; // Required for ReferenceHandler

// --- NLog: Early initialization outside try-catch for critical startup logging (BEGIN) ---
var logger = NLog.LogManager.Setup().GetCurrentClassLogger();

try
{
    logger.Debug("Application starting up and NLog initialized for early logging.");

    var builder = WebApplication.CreateBuilder(args);

    // --- NLog: Full integration with ASP.NET Core Host (BEGIN) ---
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    logger.Info("ASP.NET Core host builder setup completed. NLog configuration loaded from appsettings.json.");
    // --- NLog: Full integration with ASP.NET Core Host (END) ---

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options => // Added this block to fix JSON serialization cycles
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // Optional: You might also want camelCase naming for JSON properties, common in APIs
            // options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            // Optional: Enable pretty printing for easier readability in development
            // options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
        });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Repository and Service Registrations
    builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
    builder.Services.AddScoped<IPublisherService, PublisherService>();

    var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    }).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
        options.DefaultForbidScheme =
        options.DefaultScheme =
        options.DefaultSignInScheme =
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
                )
        };
    });

    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IBookService, BookService>();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddScoped<ITokenService, Tokenservice>();

    builder.Services.AddTransient<IEmailService , SmtpEmailService>();
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var loggerForSeeding = serviceProvider.GetRequiredService<ILogger<Program>>(); // Get logger for this scope

        // Seed roles (ensure these exist)
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                loggerForSeeding.LogInformation("Role '{RoleName}' created successfully at startup.", roleName);
            }
        }

        // Seed an Admin user (optional, but highly recommended for initial setup)
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Super", // Only if you added to ApplicationUser
                LastName = "Admin",  // Only if you added to ApplicationUser
                DateJoined = DateTime.UtcNow // Only if you added to ApplicationUser
            };
            var createAdminResult = await userManager.CreateAsync(adminUser, "Admin@123"); // <-- CHANGE THIS PASSWORD IN PRODUCTION!
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddToRoleAsync(adminUser, "User");
                loggerForSeeding.LogInformation("Admin user 'admin@example.com' created and assigned roles at startup.");
            }
            else
            {
                foreach (var error in createAdminResult.Errors)
                {
                    loggerForSeeding.LogError("Failed to create admin user at startup: {Description}", error.Description);
                }
            }
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