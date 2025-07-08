using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Specialty
{
    public class SpecialtyLinkToHospitalDto
    {
        [Required(ErrorMessage = "Specialty ID to link is required.")]
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public string HospitalId { get; set; }
        public Status Status { get; set; }
        public string SpecialtyName { get; set; }
        public string? ImageUrl { get; set; }
        public string? SpecialtyDesc { get; set; }

    }
}
