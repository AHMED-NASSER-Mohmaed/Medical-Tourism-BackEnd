using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Map Patient to its own table (e.g., "Patients")
            builder.ToTable("Patients");

            // Define the TPT relationship: Patient's PK is also its FK to User
            builder.HasBaseType<User>(); // Explicitly state its base type for TPT
            // The PK is inherited and will also serve as the FK to the base table.
            // No need to explicitly configure foreign key here if PK is shared.
        }
    }
}