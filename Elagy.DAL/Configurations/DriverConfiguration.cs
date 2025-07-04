using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasBaseType<User>(); // Driver inherits from User (TPH mapping)

            // Configure Driver-specific properties
            builder.Property(d => d.DriveLicenseLicenseNumberURL).IsRequired().HasMaxLength(500); // Increased length for URL
            builder.Property(d => d.DriveLicenseLicenseNumberId).IsRequired().HasMaxLength(255); // Corresponds to file ID
            builder.Property(d => d.YearsOfExperience).IsRequired();

            // Configure DriverStatus enum as string

            // Relationship: Driver (Many) to CarRentalAsset (One) - Driver belongs to one CarRentalAsset
            builder.HasOne(d => d.CarRentalAsset)
                   .WithMany(cra => cra.Drivers) // Make sure CarRentalAsset has ICollection<Driver>? Drivers
                   .HasForeignKey(d => d.CarRentalAssetId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting CarRentalAsset if drivers are associated
            builder.Property(d => d.DriverStatus)
       .HasConversion<string>() // stores as string (e.g., "Available")
       .IsRequired();

        }
    }
}
