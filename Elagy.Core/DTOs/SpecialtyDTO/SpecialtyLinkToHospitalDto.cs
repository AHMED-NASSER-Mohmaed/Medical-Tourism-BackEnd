using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.SpecialtyDTO
{
    public class SpecialtyLinkToHospitalDto
    {
        [Required(ErrorMessage = "Specialty ID to link is required.")]
        public int SpecialtyId { get; set; }
    }
}
