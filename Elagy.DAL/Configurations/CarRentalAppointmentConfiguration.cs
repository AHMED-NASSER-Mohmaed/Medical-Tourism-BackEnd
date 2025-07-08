using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CarRentalAppointmentConfiguration : IEntityTypeConfiguration<CarRentalAppointment>
{
    public void Configure(EntityTypeBuilder<CarRentalAppointment> builder)
    {
        builder.Property(c => c.StartingDateTime).IsRequired();
        builder.Property(c => c.EndingDateTime).IsRequired();

        builder.HasOne(c => c.CarSchedule)
              .WithMany()
               .HasForeignKey(c => c.CarScheduleId)
               .OnDelete(DeleteBehavior.Restrict); // ✅
    }
}
