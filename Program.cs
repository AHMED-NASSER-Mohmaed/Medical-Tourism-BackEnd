using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Elagy.DAL;
using Elagy.DAL.Data;
using Elagy.BL.Services; 
using Elagy.BL.Helpers; // for services like AuthService, PatientService, etc.
using Elagy.Core.Helpers; // for intefaceses like IEmailService, IJwtTokenGenerator, etc.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add for configuration access
using Microsoft.Extensions.DependencyInjection; // Add for service collection access
using Microsoft.Extensions.Logging; // Add for logging in seed
using Elagy.APIs.Initializers; // Add this namespace for DbInitializer
using System;
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;

    // Password settings (customize as needed, minimum recommended length is higher for real apps)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false; // For simplicity in dev, consider true for production
    options.Password.RequireUppercase = false; // For simplicity in dev, consider true for production
    options.Password.RequireLowercase = false; // For simplicity in dev, consider true for production

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // For email confirmation, password reset tokens

// Configure Dependency Injection for Services and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register your application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IHotelProviderService, HotelProviderService>();
builder.Services.AddScoped<IHospitalProviderService, HospitalProviderService>();
builder.Services.AddScoped<ICarRentalProviderService, CarRentalProviderService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();

// Register Helper Services
builder.Services.AddTransient<IEmailService, EmailService>(); // Transient because SmtpClient can have issues with Scoped/Singleton
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>(); // Identity's default hasher
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>(); // Your custom wrapper if needed

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure JWT Authentication (You need to add this for JWT tokens to be validated)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// Add Authorization services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireSuperAdminRole", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));
    options.AddPolicy("RequireServiceProviderRole", policy => policy.RequireRole("ServiceProvider"));
    // Add other specific policies as needed
});


var app = builder.Build();

// --- Data Seeding at Startup ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>(); // Logger for Program.cs scope

        await DbInitializer.SeedRolesAsync(userManager, roleManager, logger);
        await DbInitializer.SeedSuperAdminAsync(userManager, roleManager, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// --- End Data Seeding ---


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();