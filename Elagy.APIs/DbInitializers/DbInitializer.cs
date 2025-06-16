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
        // In Program.cs or Startup.cs, during application initialization
        // after services are built and database is ensured
        public static async Task SeedRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {

            foreach (string roleName in Enum.GetNames(typeof(RoleApps)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
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