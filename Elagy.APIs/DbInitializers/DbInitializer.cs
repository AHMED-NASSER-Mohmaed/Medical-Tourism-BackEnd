using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace Elagy.APIs.Initializers
{
    public static class DbInitializer
    {
 
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
 
      public static async Task SeedStaticDataAsync(ApplicationDbContext context, ILogger logger)
        {
            logger.LogInformation("Starting static data seeding...");

            // List of Arab countries and their governorates (major administrative divisions)
            var arabCountries = new Dictionary<string, List<string>>
            {
                { "Egypt", new List<string> { "Cairo", "Alexandria", "Aswan", "Asyut", "Beheira", "Beni Suef", "Dakahlia", "Damietta", "Faiyum", "Gharbia", "Giza", "Ismailia", "Kafr El Sheikh", "Luxor", "Matruh", "Minya", "Monufia", "New Valley", "North Sinai", "Port Said", "Qalyubia", "Qena", "Red Sea", "Sharqia", "Sohag", "South Sinai", "Suez" } },
                { "Saudi Arabia", new List<string> { "Riyadh", "Makkah", "Madinah", "Eastern Province", "Qassim", "Asir", "Tabuk", "Hail", "Northern Borders", "Jazan", "Najran", "Al Bahah", "Al Jawf" } },
                { "United Arab Emirates", new List<string> { "Abu Dhabi", "Dubai", "Sharjah", "Ajman", "Umm Al Quwain", "Ras Al Khaimah", "Fujairah" } },
                { "Jordan", new List<string> { "Amman", "Irbid", "Zarqa", "Balqa", "Madaba", "Mafraq", "Jerash", "Ajloun", "Karak", "Tafilah", "Ma'an", "Aqaba" } },
                { "Lebanon", new List<string> { "Beirut", "Mount Lebanon", "North", "South", "Beqaa", "Nabatieh", "Akkar", "Baalbek-Hermel" } },
                { "Syria", new List<string> { "Damascus", "Aleppo", "Homs", "Hama", "Latakia", "Tartus", "Idlib", "Al-Hasakah", "Deir ez-Zor", "Raqqa", "Daraa", "As-Suwayda", "Quneitra", "Rif Dimashq" } },
                { "Iraq", new List<string> { "Baghdad", "Basra", "Nineveh", "Erbil", "Sulaymaniyah", "Diyala", "Anbar", "Kirkuk", "Babil", "Najaf", "Dhi Qar", "Wasit", "Maysan", "Muthanna", "Qadisiyyah", "Karbala", "Saladin", "Dohuk" } },
                { "Kuwait", new List<string> { "Al Asimah", "Hawalli", "Farwaniya", "Ahmadi", "Jahra", "Mubarak Al-Kabeer" } },
                { "Qatar", new List<string> { "Doha", "Al Rayyan", "Al Wakrah", "Umm Salal", "Al Khor", "Al Shamal", "Al Daayen", "Al Shahaniya" } },
                { "Bahrain", new List<string> { "Capital", "Muharraq", "Northern", "Southern" } },
                { "Oman", new List<string> { "Muscat", "Dhofar", "Musandam", "Al Batinah North", "Al Batinah South", "Al Dakhiliyah", "Al Wusta", "Al Sharqiyah North", "Al Sharqiyah South", "Al Dhahirah", "Al Buraymi" } },
                { "Palestine", new List<string> { "West Bank", "Gaza Strip", "Jerusalem", "Hebron", "Nablus", "Ramallah", "Bethlehem", "Jenin", "Tulkarm", "Qalqilya", "Salfit", "Tubas", "Jericho", "Rafah", "Khan Yunis", "Deir al-Balah", "North Gaza" } },
                { "Sudan", new List<string> { "Khartoum", "Gezira", "Red Sea", "River Nile", "Northern", "Kassala", "Blue Nile", "White Nile", "North Kordofan", "South Kordofan", "West Kordofan", "North Darfur", "South Darfur", "East Darfur", "West Darfur", "Central Darfur", "Sennar" } },
                { "Libya", new List<string> { "Tripoli", "Benghazi", "Misrata", "Zawiya", "Sabha", "Sirte", "Derna", "Al Bayda", "Al Marj", "Ghat", "Nalut", "Jufra", "Murzuq", "Wadi al Shatii", "Wadi al Hayaa", "Al Kufrah" } },
                { "Algeria", new List<string> { "Algiers", "Oran", "Constantine", "Annaba", "Blida", "Batna", "Setif", "Sidi Bel Abbes", "Tlemcen", "Bejaia", "Tizi Ouzou", "Mostaganem", "Skikda", "Biskra", "Tiaret", "Bechar", "Chlef", "Djelfa", "El Oued", "Bouira", "M'sila", "Mascara", "Ouargla", "Relizane", "Tamanrasset", "Tebessa", "Ain Defla", "Ain Temouchent", "Adrar", "Laghouat", "Guelma", "Saida", "Tipaza", "Mila", "Naama", "Khenchela", "Souk Ahras", "Tindouf", "El Bayadh", "Illizi", "Bordj Bou Arreridj", "Boumerdes", "El Tarf", "Tissemsilt", "El M'Ghair", "El Menia", "Ouled Djellal", "Bordj Baji Mokhtar", "Beni Abbes", "Timimoun", "Touggourt", "Djanet", "In Salah", "In Guezzam" } },
                { "Morocco", new List<string> { "Casablanca-Settat", "Rabat-Salé-Kénitra", "Fès-Meknès", "Marrakesh-Safi", "Tanger-Tetouan-Al Hoceima", "Souss-Massa", "Beni Mellal-Khénifra", "Oriental", "Drâa-Tafilalet", "Guelmim-Oued Noun", "Laâyoune-Sakia El Hamra", "Dakhla-Oued Ed-Dahab" } },
                { "Tunisia", new List<string> { "Tunis", "Ariana", "Ben Arous", "Manouba", "Nabeul", "Zaghouan", "Bizerte", "Beja", "Jendouba", "Kef", "Siliana", "Sousse", "Monastir", "Mahdia", "Sfax", "Kairouan", "Kasserine", "Sidi Bouzid", "Gabes", "Medenine", "Tataouine", "Gafsa", "Tozeur", "Kebili" } },
                { "Mauritania", new List<string> { "Adrar", "Assaba", "Brakna", "Dakhlet Nouadhibou", "Gorgol", "Guidimaka", "Hodh Ech Chargui", "Hodh El Gharbi", "Inchiri", "Nouakchott-Nord", "Nouakchott-Ouest", "Nouakchott-Sud", "Tagant", "Tiris Zemmour", "Trarza" } },
                { "Comoros", new List<string> { "Grande Comore", "Anjouan", "Mohéli" } },
                { "Djibouti", new List<string> { "Djibouti", "Ali Sabieh", "Arta", "Dikhil", "Obock", "Tadjourah" } },
                { "Somalia", new List<string> { "Banadir", "Bari", "Bay", "Galguduud", "Gedo", "Hiiraan", "Lower Juba", "Middle Juba", "Mudug", "Nugal", "Sanaag", "Shabeellaha Dhexe", "Shabeellaha Hoose", "Sool", "Togdheer", "Woqooyi Galbeed", "Awdal" } },
                { "Yemen", new List<string> { "Aden", "Abyan", "Al Bayda", "Al Hudaydah", "Al Jawf", "Al Mahrah", "Al Mahwit", "Amran", "Dhamar", "Hadhramaut", "Hajjah", "Ibb", "Lahij", "Ma'rib", "Raymah", "Saada", "Sanaa", "Shabwah", "Socotra", "Taiz" } }
            };

            // Seed Countries
            foreach (var country in arabCountries.Keys)
            {
                if (!await context.Countries.AnyAsync(c => c.Name == country))
                {
                    context.Countries.Add(new Country { Name = country });
                }
            }
            await context.SaveChangesAsync();

            // Fetch all countries from DB for ID mapping
            var dbCountries = await context.Countries.ToListAsync();
            var countryNameToId = dbCountries.ToDictionary(c => c.Name, c => c.Id);

            // Seed Governorates
            foreach (var kvp in arabCountries)
            {
                var countryName = kvp.Key;
                var governorates = kvp.Value;
                if (!countryNameToId.TryGetValue(countryName, out int countryId))
                {
                    logger.LogError($"Country '{countryName}' not found in DB after seeding.");
                    continue;
                }

                foreach (var gov in governorates)
                {
                    if (!await context.Governaties.AnyAsync(g => g.Name == gov && g.CountryId == countryId))
                    {
                        context.Governaties.Add(new Governorate { Name = gov, CountryId = countryId });
                    }
                }
            }
            await context.SaveChangesAsync();

            logger.LogInformation("Static data seeding for all Arab countries and their governorates completed.");
        }

      
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
                    Status = Status.Active,  // Set to active for seed
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
