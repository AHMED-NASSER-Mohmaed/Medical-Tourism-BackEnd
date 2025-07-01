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
    public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
    {
        public void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            // Primary Key
            builder.HasKey(ri => ri.Id);

            // Configure properties
            builder.Property(ri => ri.ImageId)
                   .IsRequired()
                   .HasMaxLength(255); 

            builder.Property(ri => ri.ImageURL)
                   .IsRequired()
                   .HasMaxLength(500);

           
          
            builder.HasOne(ri => ri.Room)
                   .WithMany(r => r.RoomImages) 
                   .HasForeignKey(ri => ri.RoomId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }

}
