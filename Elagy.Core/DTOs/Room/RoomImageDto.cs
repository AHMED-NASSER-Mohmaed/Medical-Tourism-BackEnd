using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Room
{
    public class RoomImageDto
    {
        public int Id { get; set; }
        [Required]
        public string ImageId { get; set; } 
        [Required]
        public string ImageURL { get; set; } 

    }
}
