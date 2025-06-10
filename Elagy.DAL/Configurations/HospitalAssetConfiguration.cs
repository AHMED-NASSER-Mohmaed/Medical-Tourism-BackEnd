using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HospitalAssetConfiguration : IEntityTypeConfiguration<HospitalAsset>
    {
        public void Configure(EntityTypeBuilder<HospitalAsset> builder)
        {
            // Map HospitalAsset to its own table (e.g., "HospitalAssets")
            builder.ToTable("HospitalAssets");

            // Define the TPT relationship: HospitalAsset's PK is also its FK to ServiceAsset
            builder.HasBaseType<ServiceAsset>(); // Explicitly state its base type for TPT

            // ... (rest of HospitalAsset specific property configurations) ...
            builder.Property(ha => ha.NumberOfDepartments).IsRequired();
            builder.Property(ha => ha.HasEmergencyRoom).IsRequired();
            builder.Property(ha => ha.IsTeachingHospital).IsRequired();
            builder.Property(ha => ha.EmergencyServices).IsRequired();
        }
    }
}