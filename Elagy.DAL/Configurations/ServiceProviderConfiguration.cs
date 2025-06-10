using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
            // Map ServiceProvider to its own table (e.g., "ServiceProviders")
            builder.ToTable("ServiceProviders");

            // Define the TPT relationship: ServiceProvider's PK is also its FK to User
            builder.HasBaseType<User>(); // Explicitly state its base type for TPT

            // One-to-one relationship with ServiceAsset (remains the same)
            builder.HasOne(sp => sp.ServiceAsset)
                   .WithOne(sa => sa.ServiceProvider)
                   .HasForeignKey<ServiceAsset>(sa => sa.Id) // ServiceAsset's PK is also its FK to ServiceProvider
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}