using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Configurations
{
    public class HotelAssetImageConfiguration : IEntityTypeConfiguration<HotelAssetImage>
    {
        public void Configure(EntityTypeBuilder<HotelAssetImage> builder)
        {
            builder.HasKey(hai => hai.Id);

            builder.Property(hai => hai.ImageId).IsRequired().HasMaxLength(255);
            builder.Property(hai => hai.ImageURL).IsRequired().HasMaxLength(500);
 
            builder.HasOne(hai => hai.HotelAsset)
                   .WithMany(ha => ha.HotelAssetImages)
                   .HasForeignKey(hai => hai.HotelAssetId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
