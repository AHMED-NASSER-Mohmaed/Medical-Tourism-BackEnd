using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.CreatedAt)
                   .HasColumnType("datetime")
                   .IsRequired();

            builder.Property(p => p.PaymentConfirmedAt)
                   .HasColumnType("datetime");

            builder.Property(p => p.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(p => p.PatientId)
                   .IsRequired();

            // Relationship with Patient
            builder.HasOne(p => p.Patient)
                   .WithMany(p => p.Packages) // Make sure this exists in `Patient`
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Appointments
            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Package)
                   .HasForeignKey(a => a.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
