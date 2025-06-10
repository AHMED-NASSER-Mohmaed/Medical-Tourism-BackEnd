using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HotelAssetConfiguration : IEntityTypeConfiguration<HotelAsset>
    {
        public void Configure(EntityTypeBuilder<HotelAsset> builder)
        {
            // Map HotelAsset to its own table (e.g., "HotelAssets")
            builder.ToTable("HotelAssets");

            // Define the TPT relationship: HotelAsset's PK is also its FK to ServiceAsset
            builder.HasBaseType<ServiceAsset>(); // Explicitly state its base type for TPT

            // ... (rest of HotelAsset specific property configurations) ...
            builder.Property(ho => ho.StarRating).IsRequired(false);
            builder.Property(ho => ho.HasPool).IsRequired(false);
            builder.Property(ho => ho.HasRestaurant).IsRequired(false);
        }
    }
}