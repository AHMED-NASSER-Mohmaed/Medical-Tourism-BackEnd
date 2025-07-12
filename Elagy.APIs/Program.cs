using Elagy.APIs.Initializers; // Add this namespace for DbInitializer
using Elagy.BL.Helpers; // for services like AuthService, PatientService, etc.
using Elagy.BL.Services;
using Elagy.Core.Entities;
using Elagy.Core.Helpers; // for intefaceses like IEmailService, IJwtTokenGenerator, etc.
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Elagy.Core.Temps;
using Elagy.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add for configuration access
using Microsoft.Extensions.DependencyInjection; // Add for service collection access
using Microsoft.Extensions.Logging; // Add for logging in seed
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Stripe;
using Stripe.Checkout;
using ReportProPDF;
using Elagy.DAL.Repositories;
using System.Text.Json.Serialization;
using HtmlRendererAsPdf.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

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
    options.Password.RequireUppercase = false;       // For simplicity in dev, consider true for production
    options.Password.RequireLowercase = false;       // For simplicity in dev, consider true for production

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // For email confirmation, password reset tokens

// --- Customize Email Confirmation Token Lifespan Here ---
// The default email confirmation token provider uses DataProtectorTokenProviderOptions.
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromDays(3); 
});

// Configure Dependency Injection for Services and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageProfile, ImageProfile>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IHotelProviderService, HotelProviderService>();
builder.Services.AddScoped<IHospitalProviderService, HospitalProviderService>();
builder.Services.AddScoped<ICarRentalProviderService, CarRentalProviderService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ISpecialtyScheduleService, SpecialtyScheduleService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<ICarDriverAssignmentService, CarDriverAssignmentService>();
builder.Services.AddScoped<IServiceProvidersWebsiteService, ServiceProvidersWebsiteService>();




builder.Services.AddScoped<IPackgeService, PackageService>();
builder.Services.AddScoped<ISpecialtyAppointmentService, SpecialtyAppointmentServcie>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IRoomScheduleService, RoomScheduleService>();

builder.Services.AddScoped<IRoomAppointmentService, RoomAppointmentService>();

builder.Services.AddScoped<SessionService>();










// Register Helper Services
builder.Services.AddTransient<IEmailService, EmailService>(); // Transient because SmtpClient can have issues with Scoped/Singleton
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>(); // Identity's default hasher
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>(); // Your custom wrapper if needed



// stripe secrete key setting 
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// RegisterSerialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });




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

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            ClaimsIdentity? claimsIdentity = context.Principal.Identity as ClaimsIdentity;

            if (claimsIdentity != null && claimsIdentity.Claims.Any())
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var securityStamp = claimsIdentity.FindFirst("SecurityStamp")?.Value;

                if (userId != null && securityStamp != null)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user == null || await userManager.GetSecurityStampAsync(user) != securityStamp)
                    {
                        // If user is not found, or SecurityStamp does not match, reject the token
                        context.Fail("Invalid Security Stamp");
                    }
                }
                else
                {
                    context.Fail("Missing required claims in token.");
                }
            }
            else
            {
                context.Fail("No claims found in token.");
            }
        }
    };
});


// Add Authorization services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));
    options.AddPolicy("RequireServiceProviderRole", policy => policy.RequireRole("ServiceProvider"));
    // Add other specific policies as needed
});


//////////////////////

// Inside Program.cs, after builder.Services.AddControllers(); and other services.Add...
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()    // Allows any origin (e.g., any domain)
                   .AllowAnyHeader()    // Allows any headers in the request
                   .AllowAnyMethod();   // Allows any HTTP methods (GET, POST, PUT, DELETE, etc.)
        });
});


/////////////////////



builder.Services.Configure<ImageKitSettings>(builder.Configuration.GetSection(ImageKitSettings.ImageKitSectionName));


builder.Services.AddHttpClient<IFileStorageService, ImageKitFileStorageService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(3); // Example: 5 minutes for large uploads
});
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddPolicy("IpUploadLimit", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2, // Max 2 requests per minute per IP
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // No queueing, reject immediately
            })
        );

    rateLimiterOptions.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests from this IP. Please try again later.", cancellationToken);
        return ValueTask.CompletedTask;
    };
});

builder.Services.AddControllersWithViews();


// build Report Service
builder.Services.AddReportProPDF();
builder.Services.AddReportHtmlToAsPdf();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Resolve required services from the created scope
        var appContext = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>(); // Logger for Program.cs scope

        logger.LogInformation("Starting database seeding process...");

        // Ensure database is created/migrated. This is a common practice before seeding.
        // If you handle migrations separately (e.g., via CLI or deployment scripts),
        // you might remove this line.
        await appContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied.");

        //Execute seeding methods in a logical order
        //await DbInitializer.SeedRoles(userManager, roleManager, logger);
        //await DbInitializer.SeedSuperAdminAsync(userManager, roleManager, logger);
        //await DbInitializer.SeedStaticDataAsync(appContext, logger);

        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        // Catch any exceptions during seeding and log them
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}













// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();