using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
{
    public void Configure(EntityTypeBuilder<Governorate> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(g => g.Country)
            .WithMany(c => c.Governorates)
            .HasForeignKey(g => g.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
