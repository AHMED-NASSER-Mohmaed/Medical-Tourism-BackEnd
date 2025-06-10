using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Elagy.DAL.Configurations
{
    public class CarRentalAssetConfiguration : IEntityTypeConfiguration<CarRentalAsset>
    {
        public void Configure(EntityTypeBuilder<CarRentalAsset> builder)
        {
            // Map CarRentalAsset to its own table (e.g., "CarRentalAssets")
            builder.ToTable("CarRentalAssets");

            // Define the TPT relationship: CarRentalAsset's PK is also its FK to ServiceAsset
            builder.HasBaseType<ServiceAsset>(); // Explicitly state its base type for TPT

            // ... (rest of CarRentalAsset specific property configurations, including JSON conversions) ...
            builder.Property(cra => cra.OperationalAreas).IsRequired(false).HasMaxLength(500);

            builder.Property(e => e.VehicleType)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));

            builder.Property(e => e.Transmission)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));

            builder.Property(e => e.FuelType)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));

            builder.Property(e => e.RentalPolicies)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));

            builder.Property(e => e.AdditionalServices)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));

            builder.Property(e => e.CarFeatures)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))), c => c.ToArray()));
        }
    }
}