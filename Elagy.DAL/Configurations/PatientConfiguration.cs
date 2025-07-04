using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
 
 
             
            builder.Property(p => p.BloodGroup).HasMaxLength(10).IsRequired(false);
            builder.Property(p => p.Height).HasColumnType("real").IsRequired(false);
            builder.Property(p => p.Weight).HasColumnType("real").IsRequired(false);

            builder.HasMany(p => p.Packages)
            .WithOne(pkg => pkg.Patient)
            .HasForeignKey(pkg => pkg.PatientId)
            .OnDelete(DeleteBehavior.Restrict);  


        }
    }
}