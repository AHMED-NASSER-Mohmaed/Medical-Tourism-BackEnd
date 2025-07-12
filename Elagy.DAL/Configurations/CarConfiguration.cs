using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(c => c.Id);


            builder.Property(c => c.FactoryMake).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ModelName).IsRequired().HasMaxLength(50); // ModelName in DTO
            builder.Property(c => c.ModelYear).IsRequired(); // ModelYear in DTO
            builder.Property(c => c.Capacity).IsRequired();




            builder.Property(c => c.PricePerDay).HasColumnType("decimal(18,2)").IsRequired();;
        
            
            builder.Property(c => c.IsAvailable).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(1000).IsRequired(false);

            // Enums stored as strings
            builder.Property(c => c.Type).HasConversion<int>().HasMaxLength(50).IsRequired(); // CarType
            builder.Property(c => c.Status).HasConversion<int>().HasMaxLength(50).IsRequired(); // CarStatus
            builder.Property(c => c.Transmission).HasConversion<int>().HasMaxLength(50).IsRequired(); // TransmissionType
            builder.Property(c => c.FuelType).HasConversion<int>().HasMaxLength(50).IsRequired(); // FuelType

            // Configure CarImages collection
            builder.HasMany(c => c.CarImages)
                   .WithOne(ci => ci.Car)
                   .HasForeignKey(ci => ci.CarId)
                   .OnDelete(DeleteBehavior.Cascade); // If car deleted, its images delete



            // Relationship: Car → CarRentalSchedules
            builder.HasMany(c => c.carRentalSchedules)
                   .WithOne(s => s.Car)
                   .HasForeignKey(s => s.CarId)
                   .OnDelete(DeleteBehavior.Cascade);


        }


    }
}
