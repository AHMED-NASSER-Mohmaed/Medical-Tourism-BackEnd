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

        public DbSet<Patient> Patients { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }

        public DbSet<ServiceAsset> ServiceAssets { get; set; }
        public DbSet<CarRentalAsset> CarRentalAssets { get; set; }
        public DbSet<HospitalAsset> HospitalAssets { get; set; }
        public DbSet<HotelAsset> HotelAssets { get; set; }

        // NEW: DbSet for ImageKit temporary files
        public DbSet<ImageKitTempFile> ImageKitTempFiles { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations for TPT
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new PatientConfiguration());
            builder.ApplyConfiguration(new ServiceProviderConfiguration());
            builder.ApplyConfiguration(new SuperAdminConfiguration());

            builder.ApplyConfiguration(new ServiceAssetConfiguration());
            builder.ApplyConfiguration(new CarRentalAssetConfiguration());
            builder.ApplyConfiguration(new HospitalAssetConfiguration());
            builder.ApplyConfiguration(new HotelAssetConfiguration());

             builder.ApplyConfiguration(new ImageKitTempFileConfiguration());

            builder.Entity<Patient>().Property(p => p.Height).HasColumnType("real");
            builder.Entity<Patient>().Property(p => p.Weight).HasColumnType("real");
        }
    }
}