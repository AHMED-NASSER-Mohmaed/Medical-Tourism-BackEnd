using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Linq;

namespace Elagy.DAL.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Price)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(r => r.MaxOccupancy)
                   .IsRequired();

            builder.Property(r => r.IsAvailable)
                   .IsRequired();

            builder.Property(r => r.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(r => r.RoomNumber)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(r => r.FloorNumber)
                   .IsRequired();

            builder.Property(r => r.HasBalcony)
                   .IsRequired();

            builder.Property(r => r.IncludesBreakfast)
                   .IsRequired();

            // Enum conversions
            builder.Property(r => r.RoomType)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(r => r.ViewType)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(r => r.Status)
                   .HasConversion<int>()
                   .IsRequired();

            // JSON conversion for Amenities
            builder.Property(r => r.Amenities)
                   .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<string[]>(v) ?? new string[0],
                        new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToArray()
                        )
                    )
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);

            // Relationship: Room → HotelAsset
            builder.HasOne(r => r.HotelAsset)
                   .WithMany(h => h.Rooms)
                   .HasForeignKey(r => r.HotelAssetId)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascade cycles

            // Relationship: Room → RoomSchedules
            builder.HasMany(r => r.RoomSchedules)
                   .WithOne(rs => rs.Room)
                   .HasForeignKey(rs => rs.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Room → RoomImages
            builder.HasMany(r => r.RoomImages)
                   .WithOne(ri => ri.Room)
                   .HasForeignKey(ri => ri.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
