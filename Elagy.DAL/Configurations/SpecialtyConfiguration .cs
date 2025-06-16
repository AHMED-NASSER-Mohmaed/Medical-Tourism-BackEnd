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
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            // Map Specialty to its own table
            builder.ToTable("Specialties");

            // Configure primary key (Id is already inferred by convention, but explicit for clarity)
            builder.HasKey(s => s.Id);

            // Configure properties
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).IsRequired(false).HasMaxLength(500);


        }
    }
}
