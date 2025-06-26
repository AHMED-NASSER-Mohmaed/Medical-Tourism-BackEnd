using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elagy.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("AspNetUsers");

            // table per hirarichy 
            builder.HasDiscriminator(u => u.UserType)
                .HasValue<Patient>(UserType.Patient)
                .HasValue<ServiceProvider>(UserType.ServiceProvider)
                .HasValue<SuperAdmin>(UserType.SuperAdmin);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.ImageId).HasMaxLength(100).IsRequired(false);
            builder.Property(u => u.ImageURL).HasMaxLength(1024).IsRequired(false);
            builder.Property(u => u.Gender)
                .HasConversion<int>()
                .IsRequired();


             
             
            builder.Property(u => u.Phone).HasMaxLength(20).IsRequired(true);
            builder.Property(u => u.DateOfBirth).IsRequired(true);
            builder.Property(u => u.Status)
                   .HasConversion<int>()
                   .IsRequired();
            builder.Property(u => u.UserType)
                   .HasConversion<int>()
                   .IsRequired();



            builder.Property(u => u.Address).HasMaxLength(500).IsRequired(true);
            builder.Property(u => u.City).HasMaxLength(100).IsRequired(false);


            builder.HasOne(u => u.Governorate)
            .WithMany() 
            .HasForeignKey(u => u.GovernorateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);



            builder.Property(u => u.Gender)
            .HasConversion<int>()
            .IsRequired();


        }
    }
}