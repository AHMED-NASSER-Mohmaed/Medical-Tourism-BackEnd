using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class CarRentalScheduleConfiguration : IEntityTypeConfiguration<CarSchedule>
    {
        public void Configure(EntityTypeBuilder<CarSchedule> builder)
        {
            builder.ToTable("CarRentalSchedules");

            builder.HasKey(crs => crs.Id);

            // Handle DateOnly mapping (stored as 'date' in SQL Server)
            builder.Property(crs => crs.StartingDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(crs => crs.EndingDate)
                   .HasColumnType("date")
                   .IsRequired();

            // Relationship with Car
            builder.HasOne(crs => crs.Car)
                   .WithMany(c => c.carRentalSchedules) // Ensure Car entity has ICollection<CarRentalSchedule> Schedules
                   .HasForeignKey(crs => crs.CarId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with CarRentalAppointments
            builder.HasMany(crs => crs.CarRentalAppointments)
                   .WithOne(a => a.CarRentalSchedule)
                   .HasForeignKey(a => a.CarRentalScheduleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
