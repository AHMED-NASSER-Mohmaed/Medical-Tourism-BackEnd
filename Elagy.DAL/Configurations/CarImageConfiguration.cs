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
    public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
    {
        public void Configure(EntityTypeBuilder<CarImage> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.ImageId).IsRequired().HasMaxLength(255);
            builder.Property(ci => ci.ImageURL).IsRequired().HasMaxLength(500);

        }
    }
}
