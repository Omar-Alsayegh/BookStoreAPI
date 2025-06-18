using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookStoreApi.Extensions
{
    public static class SeedDataExtensions
    {
        public static async Task SeedRolesAndAdminUserAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var loggerForSeeding = serviceProvider.GetRequiredService<ILogger<Program>>();

                // Seed roles
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        loggerForSeeding.LogInformation("Role '{RoleName}' created successfully at startup.", roleName);
                    }
                }

                // Seed an Admin user
                var adminUser = await userManager.FindByEmailAsync("admin@example.com");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@example.com",
                        Email = "admin@example.com",
                        FirstName = "Super",
                        LastName = "Admin",
                        DateJoined = DateTime.UtcNow
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
        }
    }
}
