using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class RoomScheduleConfiguration : IEntityTypeConfiguration<RoomSchedule>
    {
        public void Configure(EntityTypeBuilder<RoomSchedule> builder)
        {
            builder.ToTable("RoomSchedules");

            builder.HasKey(rs => rs.Id);

            builder.Property(rs => rs.StartDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(rs => rs.EndDate)
                   .HasColumnType("date")
                   .IsRequired();

            // Enum conversion
            builder.Property(rs => rs.RoomscheduleStatus)
                   .HasConversion<int>()
                   .IsRequired();

            // Foreign key relationship
            builder.HasOne(rs => rs.Room)
                   .WithMany(r => r.RoomSchedules) // make sure Room has `ICollection<RoomSchedule> RoomSchedules`
                   .HasForeignKey(rs => rs.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.TotalPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();
        }
    }
}
