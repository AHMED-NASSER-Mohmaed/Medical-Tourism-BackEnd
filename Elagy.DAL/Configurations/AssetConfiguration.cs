using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Elagy.DAL.Configurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            // Map Asset to its own base table (e.g., "Assets").
            // Using "Assets" as a more generic name since it's the base for different types.
            // If you intend to keep the old "ServiceAssets" table name, adjust accordingly.
            builder.ToTable("Assets");

            // Primary Key setup: Id matches ServiceProvider's Id (shared PK for the one-to-one)
            // It's manually assigned by the ServiceProvider.Id, so ValueGeneratedNever() is appropriate.
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedNever().HasMaxLength(450); // Id from IdentityUser is nvarchar(450) by default

            // Common properties for all Assets
            builder.Property(a => a.Name).IsRequired().HasMaxLength(250); // Renamed from AssetName in old config
            builder.Property(a => a.Description).HasMaxLength(1000).IsRequired(false);
            builder.Property(a => a.CredentialDocURL).HasMaxLength(1024).IsRequired(false); // New property
            builder.Property(a => a.CredentialDocId).HasMaxLength(250).IsRequired(false); // New property
            builder.Property(a => a.Email).HasMaxLength(250).IsRequired(false);
            builder.Property(a => a.AcquisitionDate).IsRequired(); // Defaulted to DateTime.Now in entity
            builder.Property(a => a.VerificationNotes).HasMaxLength(500).IsRequired(false); // Renamed from VerifiedCountryCode in old config

            // Location Properties
            builder.Property(a => a.Latitude).HasColumnType("real").IsRequired(); // Assuming Latitude/Longitude are always required
            builder.Property(a => a.Longitude).HasColumnType("real").IsRequired(); // Assuming Latitude/Longitude are always required
            builder.Property(a => a.LocationDescription).HasMaxLength(500).IsRequired(false);

            // String Array Enums (using JSON conversion similar to your old config)
            builder.Property(a => a.Facilities)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            builder.Property(a => a.LanguagesSupported)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()), // Convert enum array to string array for JSON
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (Language)Enum.Parse(typeof(Language), s)).ToArray(), // Convert back to enum array
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Language[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));


            // TimeOnly Properties (requires conversion to TimeSpan for database storage)
            builder.Property(a => a.OpeningTime)
                .HasConversion(
                    v => v.ToTimeSpan(), // Convert TimeOnly to TimeSpan
                    v => TimeOnly.FromTimeSpan(v)); // Convert TimeSpan back to TimeOnly

            builder.Property(a => a.ClosingTime)
                .HasConversion(
                    v => v.ToTimeSpan(), // Convert TimeOnly to TimeSpan
                    v => TimeOnly.FromTimeSpan(v)); // Convert TimeSpan back to TimeOnly

 
            builder.Property(a => a.AssetType)
                   .HasConversion<string>() // Store enum as string in DB for readability
                   .HasMaxLength(50); // Adjust max length based on your longest enum name

            // One-to-one relationship with ServiceProvider (navigation property)
            // This side will define the foreign key.
            builder.HasOne(a => a.ServiceProvider)
                   .WithOne(sp => sp.ServiceAsset)
                   .HasForeignKey<Asset>(a => a.Id) // Asset's PK is also its FK to ServiceProvider
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}