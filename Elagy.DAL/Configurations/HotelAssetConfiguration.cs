using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HotelAssetConfiguration : IEntityTypeConfiguration<HotelAsset>
    {
        public void Configure(EntityTypeBuilder<HotelAsset> builder)
        {
            // Map HotelAsset to its own table ("HotelAssets")
            builder.ToTable("HotelAssets");

            // Define the TPT relationship: HotelAsset's PK is also its FK to Asset
            builder.HasBaseType<Asset>(); // Explicitly state its base type for TPT

            // Property configurations for HotelAsset specific properties
            builder.Property(ho => ho.StarRating).IsRequired(false); // Nullable int
            builder.Property(ho => ho.HasPool).IsRequired(false);    // Nullable bool
            builder.Property(ho => ho.HasRestaurant).IsRequired(false); // Nullable bool
        }
    }
}