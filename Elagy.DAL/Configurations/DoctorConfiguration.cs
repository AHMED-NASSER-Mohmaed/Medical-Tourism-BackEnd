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
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
      
            // Explicitly state its base type for TPT
            // This means Doctor's primary key will also be a foreign key to the AspNetUsers table.
            builder.HasBaseType<User>();

            // Configure specific properties for Doctor
            builder.Property(d => d.MedicalLicenseNumberURL).IsRequired().HasMaxLength(500);
            builder.Property(d => d.MedicalLicenseNumberId).IsRequired().HasMaxLength(500);
            builder.Property(d => d.YearsOfExperience).IsRequired();
            builder.Property(d => d.Bio).IsRequired(false).HasMaxLength(1000);
            builder.Property(d => d.Qualification).IsRequired(false).HasMaxLength(100);

            // This is the foreign key representing the doctor's "account" for that specific hospital and specialty.
            builder.HasOne(d => d.HospitalSpecialty)
                   .WithMany(hs => hs.Doctors)
                   .HasForeignKey(d => d.HospitalSpecialtyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of HospitalSpecialty if doctors are linked
        }
    }
}
