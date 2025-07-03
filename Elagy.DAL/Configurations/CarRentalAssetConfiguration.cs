using Elagy.Core.Entities;
using Elagy.Core.Enums;
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
            builder.ToTable("CarRentalAssets"); // Correct: maps to its own table

            builder.HasBaseType<Asset>(); // Correct: explicitly states its base type for TPT

            /*// Property configurations for CarRentalAsset specific properties
            builder.Property(cra => cra.OperationalAreas)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()),
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (Governorate)Enum.Parse(typeof(Governorate), s)).ToArray(),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Governorate[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));*/

            builder.Property(cra => cra.FuelTypes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()),
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (FuelType)Enum.Parse(typeof(FuelType), s)).ToArray(),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<FuelType[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

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

            builder.Property(cra => cra.Transmission)
                   .HasConversion<string>()
                   .HasMaxLength(50);



            // CarRentalAsset (One) to Car (Many)
            builder.HasMany(cra => cra.Cars)
                   .WithOne(c => c.CarRentalAsset)
                   .HasForeignKey(c => c.CarRentalAssetId)
                   .OnDelete(DeleteBehavior.Cascade); // If rental asset is deleted, its cars are deleted

            // CarRentalAsset (One) to CarRentalAssetImage (Many)
            builder.HasMany(cra => cra.CarRentalAssetImages)
                   .WithOne(crai => crai.CarRentalAsset)
                   .HasForeignKey(crai => crai.CarRentalAssetId)
                   .OnDelete(DeleteBehavior.Cascade); // If rental asset is deleted, its images are deleted

            // CarRentalAsset (One) to Driver (Many)
            builder.HasMany(cra => cra.Drivers)
                   .WithOne(d => d.CarRentalAsset)
                   .HasForeignKey(d => d.CarRentalAssetId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cra => cra.CarRentalAssetImages)
                  .WithOne(cd => cd.CarRentalAsset)
                  .HasForeignKey(cd => cd.CarRentalAssetId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}