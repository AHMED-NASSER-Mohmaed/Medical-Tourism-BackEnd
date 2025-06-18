using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class SuperAdminConfiguration : IEntityTypeConfiguration<SuperAdmin>
    {
        public void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {
             builder.ToTable("SuperAdmins");

             builder.HasBaseType<User>(); // Explicitly state its base type for TPT

            // Property configurations for SuperAdmin specific properties
            builder.Property(sa => sa.Docs).HasMaxLength(1024).IsRequired(false); // Assuming Docs is a URL or path
        }
    }
}