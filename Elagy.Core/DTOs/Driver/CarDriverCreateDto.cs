using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Driver
{
    public class CarDriverCreateDto
    {
        [Required] public int CarId { get; set; }
        [Required] public string DriverId { get; set; }

    }
}
