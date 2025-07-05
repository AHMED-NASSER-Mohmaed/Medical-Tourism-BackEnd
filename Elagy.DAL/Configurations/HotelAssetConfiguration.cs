using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class HotelAssetConfiguration : IEntityTypeConfiguration<HotelAsset>
    {
        public void Configure(EntityTypeBuilder<HotelAsset> builder)
        {
            builder.ToTable("HotelAssets"); // Correct: maps to its own table

            builder.HasBaseType<Asset>(); // Correct: explicitly states its base type for TPT

            // Property configurations for HotelAsset specific properties
            builder.Property(ho => ho.StarRating).IsRequired(false);
            builder.Property(ho => ho.HasPool).IsRequired(false);
            builder.Property(ho => ho.HasRestaurant).IsRequired(false);

            builder.HasMany(ho => ho.HotelAssetImages)
                  .WithOne(hai => hai.HotelAsset)
                  .HasForeignKey(hai => hai.HotelAssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}