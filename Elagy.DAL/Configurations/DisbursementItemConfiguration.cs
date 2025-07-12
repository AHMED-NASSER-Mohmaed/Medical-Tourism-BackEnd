using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.Infrastructure.Configurations
{
    public class DisbursementItemConfiguration : IEntityTypeConfiguration<DisbursementItem>
    {
        public void Configure(EntityTypeBuilder<DisbursementItem> builder)
        {
            builder.ToTable("DisbursementItems");

            builder.HasKey(di => di.Id);

            builder.Property(di => di.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(di => di.CreatedAt)
                   .HasColumnType("datetime")
                   .IsRequired();

            // Relationship with Disbursement
            builder.HasOne(di => di.Disbursement)
                   .WithMany(d => d.DisbursementItems)
                   .HasForeignKey(di => di.DisbursementId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(di => di.Appointment)
                       .WithOne()
                       .HasForeignKey<DisbursementItem>(di => di.AppointmentId)
                       .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
