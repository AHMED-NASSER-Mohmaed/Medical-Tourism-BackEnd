using Elagy.Core.Entities;
using Elagy.Core.Enums; // Required for enums
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
            // Map CarRentalAsset to its own table ("CarRentalAssets")
            builder.ToTable("CarRentalAssets");

            // Define the TPT relationship: CarRentalAsset's PK is also its FK to Asset
            builder.HasBaseType<Asset>(); // Explicitly state its base type for TPT

            // Property configurations for CarRentalAsset specific properties

            // Enum Array Properties (Governorate[], FuelType[]) using JSON conversion
            builder.Property(cra => cra.OperationalAreas)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()), // Convert enum array to string array for JSON
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (Governorate)Enum.Parse(typeof(Governorate), s)).ToArray(), // Convert back to enum array
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Governorate[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            builder.Property(cra => cra.FuelTypes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()), // Convert enum array to string array for JSON
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (FuelType)Enum.Parse(typeof(FuelType), s)).ToArray(), // Convert back to enum array
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<FuelType[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            // String Array Properties (Models, RentalPolicies) using JSON conversion
            builder.Property(cra => cra.Models)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            builder.Property(cra => cra.RentalPolicies)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            // Single Enum Property (TransmissionType) - stored as int by default
            // If you want to store it as a string, add HasConversion<string>()
            builder.Property(cra => cra.Transmission)
                   .HasConversion<string>() // Store enum as string in DB for readability
                   .HasMaxLength(50); // Adjust max length based on your longest enum name
        }
    }
}