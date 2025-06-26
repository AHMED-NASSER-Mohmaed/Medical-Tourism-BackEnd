using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasMany(c => c.Governorates)
            .WithOne(g => g.Country)
            .HasForeignKey(g => g.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
