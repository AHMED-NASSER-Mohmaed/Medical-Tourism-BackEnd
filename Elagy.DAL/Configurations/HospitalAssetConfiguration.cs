using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HospitalAssetConfiguration : IEntityTypeConfiguration<HospitalAsset>
    {
        public void Configure(EntityTypeBuilder<HospitalAsset> builder)
        {
            // Map HospitalAsset to its own table ("HospitalAssets")
            builder.ToTable("HospitalAssets");

            // Define the TPT relationship: HospitalAsset's PK is also its FK to Asset
            builder.HasBaseType<Asset>(); // Explicitly state its base type for TPT

            // Property configurations for HospitalAsset specific properties
            builder.Property(ha => ha.NumberOfDepartments).IsRequired();
            builder.Property(ha => ha.EmergencyServices).IsRequired(); // Renamed HasEmergencyRoom from old config
        }
    }
}