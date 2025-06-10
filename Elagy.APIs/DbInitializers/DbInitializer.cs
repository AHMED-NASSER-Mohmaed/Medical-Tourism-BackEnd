using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Elagy.APIs.Initializers
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            // Create roles if they don't exist
            string[] roleNames = { "SuperAdmin", "Patient", "ServiceProvider" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    logger.LogInformation($"Role '{roleName}' created.");
                }
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            // Seed SuperAdmin if not exists
            var superAdminEmail = "admin@elagy.com"; // Choose a strong, memorable email for your admin
            var superAdminPassword = "Secure@Admin123"; // Choose a very strong password

            if (await userManager.FindByEmailAsync(superAdminEmail) == null)
            {
                var superAdmin = new SuperAdmin
                {
                    UserName = superAdminEmail, // Often username is email
                    Email = superAdminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true, // Directly confirm for seed
                    PhoneNumberConfirmed = true,
                    Status = UserStatus.Active, // Set to active for seed
                    UserType = UserType.SuperAdmin,
                    Docs = "Initial Company Credentials" // Specific property for SuperAdmin
                };

                var result = await userManager.CreateAsync(superAdmin, superAdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                    logger.LogInformation($"SuperAdmin account '{superAdminEmail}' created and assigned 'SuperAdmin' role.");
                }
                else
                {
                    logger.LogError($"Failed to create SuperAdmin account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"SuperAdmin account '{superAdminEmail}' already exists.");
            }
        }
    }
}