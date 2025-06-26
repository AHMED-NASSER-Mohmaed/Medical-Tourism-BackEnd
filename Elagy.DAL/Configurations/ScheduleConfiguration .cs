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
            // Properties
            builder.Property(s => s.Id)
                   .ValueGeneratedOnAdd(); 

            builder.Property(s => s.Date)
                   .IsRequired(); 

            builder.Property(s => s.StartTime)
                   .IsRequired(); 

            builder.Property(s => s.EndTime)
                   .IsRequired();

            builder.Property(s => s.MaxCapacity)
                   .IsRequired(); 
            builder.Property(s => s.BookedSlots)
                   .IsRequired(); 
            builder.Property(s => s.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);


            // Relationships

            // Schedule (Many) to Doctor (One)
            builder.HasOne(s => s.Doctor)
                   .WithMany(d => d.Schedules)
                   .HasForeignKey(s => s.DoctorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); 

            // Schedule (Many) to HospitalSpecialty (One)
            builder.HasOne(s => s.HospitalSpecialty)
                   .WithMany(hs => hs.Schedules)
                   .HasForeignKey(s => s.HospitalSpecialtyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); 

            //// Schedule (One) to Appointment (Many)
            //// This relationship is also defined in AppointmentConfiguration for the other side
            //builder.HasMany(s => s.Appointments)
            //       .WithOne(a => a.Schedule)
            //       .HasForeignKey(a => a.ScheduleId)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if a schedule has appointments

            // Indexes
            // Add an index for efficient lookup of a doctor's schedules on a specific date.
            //builder.HasIndex(s => new { s.DoctorId, s.Date, s.StartTime })
            //       .IsUnique(false); // Not unique, as a doctor can have multiple schedules on a date, just different times

   
        }
    }
    }

