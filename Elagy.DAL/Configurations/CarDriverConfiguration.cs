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
    public class CarDriverConfiguration : IEntityTypeConfiguration<CarDriver>
    {
        public void Configure(EntityTypeBuilder<CarDriver> builder)
        {
            // Composite Primary Key
            builder.HasKey(cd => cd.Id);

            // Configure properties
            builder.Property(cd => cd.AssignmentDate).IsRequired();
            builder.Property(cd => cd.IsAssignedCurrent).IsRequired();
            builder.Property(cd => cd.ReleaseDate).IsRequired(false); // Make nullable in DB

            // Relationships
            builder.HasOne(cd => cd.Car)
                   .WithMany(c => c.CarDrivers)
                   .HasForeignKey(cd => cd.CarId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting car if drivers are assigned

            builder.HasOne(cd => cd.Driver)
                   .WithMany(d => d.CarDrivers)
                   .HasForeignKey(cd => cd.DriverId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting driver if cars are assigned


             builder.HasIndex(cd => new { cd.CarId, cd.DriverId }).IsUnique();
        }
    }
}
