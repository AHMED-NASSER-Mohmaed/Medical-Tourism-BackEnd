using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SpecialtyAppointmentConfiguration : IEntityTypeConfiguration<SpecialtyAppointment>
{
    public void Configure(EntityTypeBuilder<SpecialtyAppointment> builder)
    {
        builder.Property(s => s.MeetingUrl).HasMaxLength(512);
        builder.Property(s => s.ExistingTime).IsRequired();

        builder.HasOne(s => s.SpecialtySchedule)
               .WithMany(s => s.Appointments)
               .HasForeignKey(s => s.SpecialtyScheduleId)
               .OnDelete(DeleteBehavior.Restrict); // ✅
    }
}
