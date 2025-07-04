using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoomAppointmentConfiguration : IEntityTypeConfiguration<RoomAppointment>
{
    public void Configure(EntityTypeBuilder<RoomAppointment> builder)
    {
        builder.Property(r => r.CheckInDate).HasColumnType("date");
        builder.Property(r => r.CheckOutDate).HasColumnType("date");

        builder.HasOne(r => r.RoomSchedule)
               .WithMany()
               .HasForeignKey(r => r.HotelScheduleId)
               .OnDelete(DeleteBehavior.Restrict); // ✅

        builder.HasOne(r => r.Room)
               .WithMany()
               .HasForeignKey(r => r.RoomId)
               .OnDelete(DeleteBehavior.Restrict); // ✅
    }
}
