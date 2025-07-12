using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.Infrastructure.Configurations
{
    public class DisbursementConfiguration : IEntityTypeConfiguration<Disbursement>
    {
        public void Configure(EntityTypeBuilder<Disbursement> builder)
        {
            builder.ToTable("Disbursements");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DisbursementDateMonth)
                   .HasColumnType("date") // Ensure compatibility if using SQL Server
                   .IsRequired();

            builder.Property(d => d.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(d => d.GeneratedAt)
                   .HasColumnType("datetime")
                   .IsRequired();

            builder.Property(d => d.PaymentMethod)
                   .HasMaxLength(100)
                   .IsRequired();


          //  Relationship with DisbursementItems
            builder.HasMany(d => d.DisbursementItems)
                   .WithOne(di => di.Disbursement)
                   .HasForeignKey(di => di.DisbursementId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(d => d.Asset)
           .WithMany(a => a.Disbursements)
           .HasForeignKey(d => d.AssetId)
           .OnDelete(DeleteBehavior.Restrict); // or Cascade, depending on your logic



        }
    }
}
