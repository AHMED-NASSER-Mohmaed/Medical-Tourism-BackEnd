using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.price)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(a => a.Type)
               .HasConversion<int>();

        builder.Property(a => a.Status)
               .HasConversion<int>();

        builder.HasOne(a => a.Package)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PackageId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(a => a.DisbursementItem)
               .WithOne(d => d.Appointment)
               .HasForeignKey<Appointment>(a => a.DisbursementItemId)
               .OnDelete(DeleteBehavior.Restrict);  
    }
}
