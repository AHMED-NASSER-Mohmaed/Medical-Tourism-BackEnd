using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class PaymentIntentConfiguration : IEntityTypeConfiguration<PaymentIntent>
    {
        public void Configure(EntityTypeBuilder<PaymentIntent> builder)
        {
            builder.ToTable("PaymentIntents");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.StripePaymentIntentId).HasMaxLength(255);
            builder.Property(pi => pi.StripeChargeId).HasMaxLength(255);
            builder.Property(pi => pi.StripeCustomerId).HasMaxLength(255);
            builder.Property(pi => pi.StripeInvoiceId).HasMaxLength(255);
            builder.Property(pi => pi.StripePaymentMethodId).HasMaxLength(255);

            builder.Property(pi => pi.Amount).IsRequired();
            builder.Property(pi => pi.Currency).HasMaxLength(10);
            builder.Property(pi => pi.PaymentStatus).HasMaxLength(100);
            builder.Property(pi => pi.ReceiptUrl).HasMaxLength(500);
            builder.Property(pi => pi.Description).HasMaxLength(1000);

            builder.Property(pi => pi.PaymentMethodType).HasMaxLength(50);
            builder.Property(pi => pi.CardBrand).HasMaxLength(50);
            builder.Property(pi => pi.CardLast4).HasMaxLength(4);
            builder.Property(pi => pi.CardExpMonth);
            builder.Property(pi => pi.CardExpYear);

            builder.Property(pi => pi.IsCaptured);
            builder.Property(pi => pi.CapturedAt).HasColumnType("datetime");
            builder.Property(pi => pi.Refunded);
            builder.Property(pi => pi.RefundedAmount);
            builder.Property(pi => pi.FailureMessage).HasMaxLength(1000);
            builder.Property(pi => pi.StripeRawDataJson).HasColumnType("nvarchar(max)");

            builder.Property(pi => pi.CreatedAt).HasColumnType("datetime");
            builder.Property(pi => pi.UpdatedAt).HasColumnType("datetime");

            // Relationship with Package
            builder.HasOne(pi => pi.Packages)
                   .WithMany() // or .WithMany(p => p.PaymentIntents) if you add that nav property
                   .HasForeignKey(pi => pi.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
