using Elagy.Core.Entities;
using Elagy.DAL.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Elagy.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for your CONCRETE entities
        public DbSet<Patient> Patients { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }

        // DbSets for ServiceAsset hierarchy
        public DbSet<ServiceAsset> ServiceAssets { get; set; }
        public DbSet<CarRentalAsset> CarRentalAssets { get; set; }
        public DbSet<HospitalAsset> HospitalAssets { get; set; }
        public DbSet<HotelAsset> HotelAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations for TPT
            builder.ApplyConfiguration(new UserConfiguration()); // Base User configuration
            builder.ApplyConfiguration(new PatientConfiguration()); // New Patient configuration
            builder.ApplyConfiguration(new ServiceProviderConfiguration()); // Modified SP configuration
            builder.ApplyConfiguration(new SuperAdminConfiguration()); // New SuperAdmin configuration

            builder.ApplyConfiguration(new ServiceAssetConfiguration()); // Base ServiceAsset configuration
            builder.ApplyConfiguration(new CarRentalAssetConfiguration()); // Modified CarRental configuration
            builder.ApplyConfiguration(new HospitalAssetConfiguration()); // Modified Hospital configuration
            builder.ApplyConfiguration(new HotelAssetConfiguration()); // Modified Hotel configuration

            // Configure Patient's specific properties precision if needed (as done before)
            builder.Entity<Patient>().Property(p => p.Height).HasColumnType("real");
            builder.Entity<Patient>().Property(p => p.Weight).HasColumnType("real");
        }
    }
}