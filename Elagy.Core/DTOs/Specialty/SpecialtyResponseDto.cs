﻿using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Specialty
{
    public class SpecialtyResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageId { get; set; } 
        public string? ImageURL { get; set; }
        public Status Status { get; set; }
    }
}
