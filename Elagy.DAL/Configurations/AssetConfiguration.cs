using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Elagy.DAL.Configurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable("Assets"); // Base table for common Asset properties

 

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedNever().HasMaxLength(450); // Id from IdentityUser is nvarchar(450) by default

            // Common properties for all Assets
            builder.Property(a => a.Name).IsRequired().HasMaxLength(250);
            builder.Property(a => a.Description).HasMaxLength(1000).IsRequired(false);
            builder.Property(a => a.CredentialDocURL).HasMaxLength(1024).IsRequired(false);
            builder.Property(a => a.CredentialDocId).HasMaxLength(250).IsRequired(false);
            builder.Property(a => a.Email).HasMaxLength(250).IsRequired(false);
            builder.Property(a => a.VerificationNotes).HasMaxLength(500).IsRequired(false);

            // Location Properties
            builder.Property(a => a.Latitude).HasColumnType("real").IsRequired();
            builder.Property(a => a.Longitude).HasColumnType("real").IsRequired();
            builder.Property(a => a.LocationDescription).HasMaxLength(500).IsRequired(false);

            // String Array Enums (using JSON conversion)
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
                    v => JsonConvert.SerializeObject(v.Select(e => e.ToString()).ToArray()),
                    v => JsonConvert.DeserializeObject<string[]>(v).Select(s => (Language)Enum.Parse(typeof(Language), s)).ToArray(),
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Language[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, val) => HashCode.Combine(a, (val != null ? val.GetHashCode() : 0))),
                        c => c.ToArray()));

            // TimeOnly Properties (requires conversion to TimeSpan for database storage)
            builder.Property(a => a.OpeningTime)
                .HasConversion(
                    v => v.ToTimeSpan(),
                    v => TimeOnly.FromTimeSpan(v));

            builder.Property(a => a.ClosingTime)
                .HasConversion(
                    v => v.ToTimeSpan(),
                    v => TimeOnly.FromTimeSpan(v));

            // AssetType will still be a property of the base Asset class for data storage,
            // but it's not the *discriminator* for inheritance mapping in TPT.
            builder.Property(a => a.AssetType)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // One-to-one relationship with ServiceProvider
            builder.HasOne(a => a.ServiceProvider)
                   .WithOne(sp => sp.ServiceAsset)
                   .HasForeignKey<Asset>(a => a.Id) // Asset's PK is also its FK to ServiceProvider
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(a => a.Governate)
                  .WithMany()
                  .HasForeignKey(a => a.GovernateId)
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.Restrict);





        }
    }
}