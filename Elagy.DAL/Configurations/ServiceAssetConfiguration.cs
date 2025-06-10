using Elagy.Core.Entities;
using Elagy.Core.Enums; // Not needed for TPT discriminator, but other enums might be
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Elagy.DAL.Configurations
{
    public class ServiceAssetConfiguration : IEntityTypeConfiguration<ServiceAsset>
    {
        public void Configure(EntityTypeBuilder<ServiceAsset> builder)
        {
            // Map ServiceAsset to its own base table (e.g., "ServiceAssets")
            builder.ToTable("ServiceAssets");

            // No discriminator column needed for TPT

            // Primary Key setup (remains the same, will be FK for derived tables)
            builder.HasKey(sa => sa.Id);
            builder.Property(sa => sa.Id).ValueGeneratedNever();

            // ... (rest of common property configurations like AssetName, Description, Email, etc.) ...
            builder.Property(sa => sa.AssetName).IsRequired().HasMaxLength(250);
            builder.Property(sa => sa.Description).HasMaxLength(1000).IsRequired(false);
            builder.Property(sa => sa.DocsURL).IsRequired(false);
            builder.Property(sa => sa.Email).HasMaxLength(250).IsRequired(false);
            builder.Property(sa => sa.AcquisitionDate).IsRequired();
            builder.Property(sa => sa.VerificationNotes).IsRequired(false);
            builder.Property(sa => sa.VerifiedCountryCode).IsRequired(false);
            builder.Property(sa => sa.Latitude).HasColumnType("real");
            builder.Property(sa => sa.Longitude).HasColumnType("real");
            builder.Property(sa => sa.LocationDescription).HasMaxLength(500).IsRequired(false);
            builder.Property(sa => sa.OpeningHours).HasMaxLength(200).IsRequired(false);

            // JSON conversion for string arrays with ValueComparer for change tracking (remains the same)
            builder.Property(e => e.Facilities)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            builder.Property(e => e.LanguagesSupported)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));
        }
    }
}