using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class SuperAdminConfiguration : IEntityTypeConfiguration<SuperAdmin>
    {
        public void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {
            // Map SuperAdmin to its own table (e.g., "SuperAdmins")
            builder.ToTable("SuperAdmins");

            // Define the TPT relationship: SuperAdmin's PK is also its FK to User
            builder.HasBaseType<User>(); // Explicitly state its base type for TPT
        }
    }
}