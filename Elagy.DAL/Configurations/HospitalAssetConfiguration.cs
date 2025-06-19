using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HospitalAssetConfiguration : IEntityTypeConfiguration<HospitalAsset>
    {
        public void Configure(EntityTypeBuilder<HospitalAsset> builder)
        {
            builder.ToTable("HospitalAssets"); // Correct: maps to its own table

            builder.HasBaseType<Asset>(); // Correct: explicitly states its base type for TPT

            // Property configurations for HospitalAsset specific properties
            builder.Property(ha => ha.NumberOfDepartments).IsRequired();
            builder.Property(ha => ha.EmergencyServices).IsRequired();
        }
    }
}