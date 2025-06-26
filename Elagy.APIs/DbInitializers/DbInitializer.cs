using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Added for FirstOrDefaultAsync, Any
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Added for .Select() and .Any()
using System.Threading.Tasks;

namespace Elagy.APIs.Initializers
{
    public static class DbInitializer
    {
        /// <summary>
        /// Seeds application roles based on the RoleApps enum.
        /// Ensures each role exists before attempting to create it.
        /// </summary>
        /// <param name="userManager">The UserManager instance.</param>
        /// <param name="roleManager">The RoleManager instance.</param>
        /// <param name="logger">The logger for logging messages.</param>
        public static async Task SeedRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            logger.LogInformation("Starting role seeding...");
            foreach (string roleName in Enum.GetNames(typeof(RoleApps)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation($"Creating role '{roleName}'.");
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"Role '{roleName}' already exists.");
                }
            }
            logger.LogInformation("Role seeding completed.");
        }

        /// <summary>
        /// Seeds static data such as Countries and Governorates.
        /// Checks for existing data to prevent duplicate insertions.
        /// Assumes Country and Governorate entities use database-generated IDs.
        /// </summary>
        /// <param name="context">The ApplicationDbContext instance.</param>
        /// <param name="logger">The logger for logging messages.</param>
        public static async Task SeedStaticDataAsync(ApplicationDbContext context, ILogger logger)
        {
            logger.LogInformation("Starting static data seeding...");

            // Seed Countries
            if (!await context.Countries.AnyAsync()) // Use AnyAsync for async check
            {
                logger.LogInformation("No countries found. Seeding initial countries.");
                var egypt = new Country { Name = "Egypt" };
                var saudi = new Country { Name = "Saudi Arabia" };

                context.Countries.AddRange(egypt, saudi);
                await context.SaveChangesAsync(); // Save changes to populate IDs for newly added countries

                logger.LogInformation("Initial countries seeded successfully.");
            }
            else
            {
                logger.LogInformation("Countries already exist, skipping country seeding.");
            }

            // Fetch existing countries to ensure correct foreign key relationships for governorates
            // Fetching by name is safer than relying on hardcoded IDs after initial save
            var existingEgypt = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Egypt");
            var existingSaudi = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Saudi Arabia");

            if (existingEgypt == null || existingSaudi == null)
            {
                logger.LogError("Required countries (Egypt, Saudi Arabia) not found after seeding/check. Cannot seed governorates.");
                return; // Exit if base data isn't available
            }

            // Seed Governorates
            if (!await context.Governaties.AnyAsync()) // Use AnyAsync for async check
            {
                logger.LogInformation("No governorates found. Seeding initial governorates.");

                var governorates = new List<Governorate>
                {
                    new Governorate { Name = "Cairo", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Alexandria", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Aswan", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Asyut", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Beheira", CountryId = existingEgypt.Id },
                    new Governorate { Name = "BeniSuef", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Dakahlia", CountryId = existingEgypt.Id },
                    new Governorate { Name = "Riyadh", CountryId = existingSaudi.Id }
                };

                context.Governaties.AddRange(governorates);
                await context.SaveChangesAsync();
                logger.LogInformation("Initial governorates seeded successfully.");
            }
            else
            {
                logger.LogInformation("Governorates already exist, skipping governorate seeding.");
            }

            logger.LogInformation("Static data seeding completed (if not already present).");
        }

        /// <summary>
        /// Seeds a SuperAdmin user if one does not already exist.
        /// Assigns the SuperAdmin role.
        /// IMPORTANT: The password should be read from secure configuration (e.g., User Secrets, Environment Variables), NOT hardcoded.
        /// </summary>
        /// <param name="userManager">The UserManager instance.</param>
        /// <param name="roleManager">The RoleManager instance.</param>
        /// <param name="logger">The logger for logging messages.</param>
        public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            logger.LogInformation("Starting SuperAdmin seeding...");

            // IMPORTANT: In a real application, retrieve this password from a secure source
            // like User Secrets (for dev) or environment variables/Key Vault (for prod).
            // DO NOT HARDCODE SENSITIVE INFORMATION LIKE PASSWORDS IN PRODUCTION CODE.
            var superAdminEmail = "admin@elagy.com";
            var superAdminPassword = "Secure@Admin123!"; // Placeholder - Replace with a robust, configurable password!

            var existingSuperAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (existingSuperAdmin == null)
            {
                logger.LogInformation($"Attempting to create SuperAdmin account '{superAdminEmail}'.");
                var superAdmin = new SuperAdmin
                {
                    UserName = superAdminEmail, // Typically, username is the email
                    Email = superAdminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    Gender = Gender.Male,
                    EmailConfirmed = true,       // Directly confirm for seed
                    PhoneNumberConfirmed = true,
                    DateOfBirth= DateTime.UtcNow,
                    Phone="01018208958",
                    Status = UserStatus.Active,  // Set to active for seed
                    UserType = UserType.SuperAdmin,
                    Docs = "Initial Company Credentials", // Specific property for SuperAdmin
                    Address = "Default Admin Address" // <--- ADDED THIS LINE TO FIX THE ERROR
                };

                var result = await userManager.CreateAsync(superAdmin, superAdminPassword);

                if (result.Succeeded)
                {
                    // Ensure the "SuperAdmin" role exists before assigning it
                    // This is a safety check; ideally SeedRoles would run first
                    if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                        logger.LogInformation("Created 'SuperAdmin' role dynamically for initial SuperAdmin assignment.");
                    }
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                    logger.LogInformation($"SuperAdmin account '{superAdminEmail}' created and assigned 'SuperAdmin' role successfully.");
                }
                else
                {
                    logger.LogError($"Failed to create SuperAdmin account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"SuperAdmin account '{superAdminEmail}' already exists. Ensuring role is assigned.");
                // Ensure existing admin user has the SuperAdmin role
                if (!await userManager.IsInRoleAsync(existingSuperAdmin, "SuperAdmin"))
                {
                    // Ensure the "SuperAdmin" role exists if it's missing for some reason
                    if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                        logger.LogInformation("Created 'SuperAdmin' role dynamically for existing SuperAdmin assignment.");
                    }
                    await userManager.AddToRoleAsync(existingSuperAdmin, "SuperAdmin");
                    logger.LogInformation($"Ensured existing SuperAdmin account '{superAdminEmail}' has 'SuperAdmin' role.");
                }
            }
            logger.LogInformation("SuperAdmin seeding completed.");
        }
    }
}
