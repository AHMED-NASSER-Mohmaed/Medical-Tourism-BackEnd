using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Specialty
{
    public class SpecialtyUpdateDto
    {
        [Required(ErrorMessage = "Specialty name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }



    }

}
