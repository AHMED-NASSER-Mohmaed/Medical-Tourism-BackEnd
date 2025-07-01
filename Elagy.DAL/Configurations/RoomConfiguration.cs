using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;


namespace Elagy.DAL.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(r => r.MaxOccupancy).IsRequired();
            builder.Property(r => r.IsAvailable).IsRequired();
            builder.Property(r => r.Description).HasMaxLength(1000).IsRequired(false); // Make optional string
            builder.Property(r => r.RoomNumber).HasMaxLength(50).IsRequired();
            builder.Property(r => r.FloorNumber).IsRequired();
            builder.Property(r => r.HasBalcony).IsRequired();

            builder.Property(r => r.RoomType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(r => r.ViewType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

            builder.Property(r => r.IncludesBreakfast).IsRequired();

            builder.Property(r => r.Amenities)
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), 
                       v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)!
                   )
                   .IsRequired(false); 

            builder.HasOne(r => r.HotelAsset)
                   .WithMany(ha => ha.Rooms) 
                   .HasForeignKey(r => r.HotelAssetId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RoomImages)
                   .WithOne(ri => ri.Room)
                   .HasForeignKey(ri => ri.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
