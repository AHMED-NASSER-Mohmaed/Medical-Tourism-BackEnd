using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Map Patient to its own table ("Patients")
            builder.ToTable("Patients");

            // Define the TPT relationship: Patient's PK is also its FK to User
            builder.HasBaseType<User>(); // Explicitly state its base type for TPT
          

            // Property configurations for Patient specific properties
            builder.Property(p => p.BloodGroup).HasMaxLength(10).IsRequired(false); // e.g., "A+", "O-"
            builder.Property(p => p.Height).HasColumnType("real").IsRequired();
            builder.Property(p => p.Weight).HasColumnType("real").IsRequired();
        }
    }
}