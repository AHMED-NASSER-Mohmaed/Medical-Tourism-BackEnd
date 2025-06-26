using Elagy.Core.Entities;
using Elagy.DAL.Configurations; // Assuming your configurations are here
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User> // Assuming your User entity is the primary Identity user
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for your entities (optional for TPT base tables, but good practice)
    public DbSet<Asset> Assets { get; set; }
    public DbSet<CarRentalAsset> CarRentalAssets { get; set; }
    public DbSet<HospitalAsset> HospitalAssets { get; set; }
    public DbSet<HotelAsset> HotelAssets { get; set; }

    public DbSet<User> AppUsers { get; set; } // Use a different name than "Users" to avoid conflict with Identity's Users
    public DbSet<Patient> Patients { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<SuperAdmin> SuperAdmins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<HospitalSpecialty> HospitalSpecialties { get; set; }
    public DbSet<Schedule> Schedules { get; set; }


    public DbSet<Country> Countries { get; set; }
    public DbSet<Governorate> Governaties { get; set; }



    // public DbSet<ImageKitTempFile> ImageKitTempFiles { get; set; } // If you still have this entity

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // IMPORTANT: Call base.OnModelCreating for Identity configurations

        // Apply your custom configurations
        builder.ApplyConfiguration(new AssetConfiguration());
        builder.ApplyConfiguration(new CarRentalAssetConfiguration());
        builder.ApplyConfiguration(new HospitalAssetConfiguration());
        builder.ApplyConfiguration(new HotelAssetConfiguration());

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new PatientConfiguration());
        builder.ApplyConfiguration(new ServiceProviderConfiguration());
        builder.ApplyConfiguration(new SuperAdminConfiguration());
        builder.ApplyConfiguration(new SpecialtyConfiguration());
        builder.ApplyConfiguration(new HospitalSpecialtyConfiguration());
        builder.ApplyConfiguration(new DoctorConfiguration());

        builder.ApplyConfiguration(new ScheduleConfiguration());

        // builder.ApplyConfiguration(new ImageKitTempFileConfiguration()); // If you still have this entity
    }






}