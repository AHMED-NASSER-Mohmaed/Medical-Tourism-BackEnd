using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure the base User table for TPT
            builder.ToTable("AspNetUsers"); // The default table name for IdentityUser, ensure it's explicitly set.

            // No discriminator column needed for TPT

            // Configure specific properties if needed
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PhoneNumber).IsRequired(false);
            builder.Property(u => u.ImageURL).IsRequired(false);
            builder.Property(u => u.ImageId).IsRequired(false);
            builder.Property(u => u.Gender).IsRequired(false);
            builder.Property(u => u.Nationality).IsRequired(false);
            builder.Property(u => u.ZipCode).IsRequired(false);
            builder.Property(u => u.StreetNumber).IsRequired(false);
            builder.Property(u => u.Governorate).IsRequired(false);

            // Configure derived tables for TPT
            builder.HasMany<Patient>().WithOne(); // EF Core often handles this, but explicit mapping can be useful.
            builder.HasMany<ServiceProvider>().WithOne();
            builder.HasMany<SuperAdmin>().WithOne();
            builder.HasMany<Doctor>().WithOne();

        }
    }
}