using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class ImageKitTempFileConfiguration : IEntityTypeConfiguration<ImageKitTempFile>
    {
        public void Configure(EntityTypeBuilder<ImageKitTempFile> builder)
        {
            builder.ToTable("ImageKitTempFiles");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasMaxLength(100).IsRequired();

            builder.Property(t => t.OriginalFileName).HasMaxLength(255).IsRequired();
            builder.Property(t => t.ImageKitUrl).HasMaxLength(1024).IsRequired();
            builder.Property(t => t.ImageKitFilePath).HasMaxLength(500).IsRequired();
            builder.Property(t => t.UploadedDate).IsRequired();
            builder.Property(t => t.IsConfirmed).IsRequired();
        }
    }
}