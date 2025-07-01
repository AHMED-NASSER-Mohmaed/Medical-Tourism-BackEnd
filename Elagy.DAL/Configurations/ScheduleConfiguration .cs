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
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(s => s.Id);

            // Configure properties (ensure required properties are marked)
            builder.Property(s => s.DoctorId).IsRequired();
            builder.Property(s => s.HospitalSpecialtyId).IsRequired();
            builder.Property(s => s.DayOfWeekId).IsRequired();
            builder.Property(s => s.Date).IsRequired();
            builder.Property(s => s.StartTime).IsRequired();
            builder.Property(s => s.EndTime).IsRequired();
            builder.Property(s => s.MaxCapacity).IsRequired();
            builder.Property(s => s.BookedSlots).IsRequired();
            builder.Property(s => s.IsActive).IsRequired();
            builder.Property(s => s.IsRecurring).IsRequired();

            // Relationships:

            // Schedule (Many) to Doctor (One)
            // A Schedule belongs to one Doctor, and a Doctor can have many Schedules.
            builder.HasOne(s => s.Doctor)
                   .WithMany(d => d.Schedules)
                   .HasForeignKey(s => s.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict); // Or .Cascade, .NoAction, etc., based on your policy

            // Schedule (Many) to HospitalSpecialty (One)
            // A Schedule belongs to one HospitalSpecialty, and a HospitalSpecialty can have many Schedules.
            builder.HasOne(s => s.HospitalSpecialty)
                   .WithMany(hs => hs.Schedules)
                   .HasForeignKey(s => s.HospitalSpecialtyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Schedule (Many) to DayOfWeek (One)
            // A Schedule belongs to one DayOfWeek, and a DayOfWeek can have many Schedules.
            builder.HasOne(s => s.DayOfWeek)
                   .WithMany(dow => dow.Schedules)
                   .HasForeignKey(s => s.DayOfWeekId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
    }

