using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
             builder.ToTable("ServiceProviders");

             builder.HasBaseType<User>(); // Explicitly state its base type for TPT

            // Property configurations for ServiceProvider specific properties
            builder.Property(sp => sp.NationalURL).HasMaxLength(1024).IsRequired();
            builder.Property(sp => sp.NationalFeildId).HasMaxLength(250).IsRequired(); 

            
            builder.HasOne(sp => sp.ServiceAsset)
                   .WithOne(a => a.ServiceProvider) // 'a' here refers to Asset
                   .HasForeignKey<Asset>(a => a.Id) // Asset's PK (Id) is also its FK to ServiceProvider's Id
                   .IsRequired(false) // Changed to IsRequired(false) because a ServiceProvider might be created before their asset is associated
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete if ServiceProvider is deleted
        }
    }
}