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
    public class HospitalSpecialtyConfiguration : IEntityTypeConfiguration<HospitalSpecialty>
    {
        public void Configure(EntityTypeBuilder<HospitalSpecialty> builder)
        {
            // Map HospitalSpecialty to its own table (the join table)
            builder.ToTable("HospitalSpecialties");

            // Define the primary key
            builder.HasKey(hs => hs.Id);

            // Configure relationships:

            // Relationship to HospitalAsset (one-to-many from HospitalAsset to HospitalSpecialty)
            builder.HasOne(hs => hs.HospitalAsset)
                   .WithMany(ha => ha.HospitalSpecialties)
                   .HasForeignKey(hs => hs.HospitalAssetId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // If a HospitalAsset is deleted, its HospitalSpecialty entries are also deleted.

            // Relationship to Specialty (one-to-many from Specialty to HospitalSpecialty)
            builder.HasOne(hs => hs.Specialty)
                   .WithMany(s => s.HospitalSpecialties)
                   .HasForeignKey(hs => hs.SpecialtyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of a Specialty if it's used in HospitalSpecialty.

            // Add a unique constraint to ensure a specialty exists only once per hospital
            // This is crucial for the "uniqueness for the specialty in the hospital" requirement.
            builder.HasIndex(hs => new { hs.HospitalAssetId, hs.SpecialtyId })
                   .IsUnique();
        }
    }
}
