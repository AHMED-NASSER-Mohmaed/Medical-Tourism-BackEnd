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
    public class DayOfWeekConfiguration : IEntityTypeConfiguration<Core.Entities.DayOfWeek>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.DayOfWeek> builder)
        {
            builder.HasKey(dow => dow.Id);

            // Configure properties
            builder.Property(dow => dow.Name).IsRequired().HasMaxLength(50);
            builder.Property(dow => dow.ShortCode).IsRequired().HasMaxLength(10);

            // Seed initial data for DayOfWeek (essential for DayOfWeekId foreign key to work)
            builder.HasData(
                new Core.Entities.DayOfWeek{ Id = 1, Name = "Sunday", ShortCode = "SUN" },
                new Core.Entities.DayOfWeek { Id = 2, Name = "Monday", ShortCode = "MON" },
                new Core.Entities.DayOfWeek { Id = 3, Name = "Tuesday", ShortCode = "TUE" },
                new Core.Entities.DayOfWeek { Id = 4, Name = "Wednesday", ShortCode = "WED" },
                new Core.Entities.DayOfWeek { Id = 5, Name = "Thursday", ShortCode = "THU" },
                new Core.Entities.DayOfWeek { Id = 6, Name = "Friday", ShortCode = "FRI" },
                new Core.Entities.DayOfWeek { Id = 7, Name = "Saturday", ShortCode = "SAT" }
            );

        }
    }
}
