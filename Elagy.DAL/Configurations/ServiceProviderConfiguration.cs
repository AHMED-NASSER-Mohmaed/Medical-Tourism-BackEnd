using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
 
 
            // Property configurations for ServiceProvider specific properties
            builder.Property(sp => sp.NationalURL).HasMaxLength(1024).IsRequired();
            builder.Property(sp => sp.NationalFeildId).HasMaxLength(250).IsRequired();


            builder.HasOne(sp => sp.ServiceAsset)
                   .WithOne(a => a.ServiceProvider)
                   .HasForeignKey<Asset>(a => a.Id)
                   .IsRequired(false) // Changed to IsRequired(false) because a ServiceProvider might be created before their asset is associated
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}