using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CarRentalAppointmentConfiguration : IEntityTypeConfiguration<CarRentalAppointment>
{
    public void Configure(EntityTypeBuilder<CarRentalAppointment> builder)
    {
        builder.Property(c => c.StartingDate).IsRequired();
        builder.Property(c => c.EndingDate).IsRequired();

        builder.HasOne(c => c.CarSchedule)
              .WithMany()
               .HasForeignKey(c => c.CarScheduleId)
               .OnDelete(DeleteBehavior.Restrict); // ✅
    }
}
