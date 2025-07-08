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
    public class HospitalAssetImageConfiguration : IEntityTypeConfiguration<HospitalAssetImage>
    {
        public void Configure(EntityTypeBuilder<HospitalAssetImage> builder)
        {
            builder.HasKey(hai => hai.Id);

            builder.Property(hai => hai.ImageId).IsRequired().HasMaxLength(255);
            builder.Property(hai => hai.ImageURL).IsRequired().HasMaxLength(500);


            builder.HasOne(hai => hai.HospitalAsset)
                   .WithMany(ha => ha.HospitalAssetImages)
                   .HasForeignKey(hai => hai.HospitalAssetId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
