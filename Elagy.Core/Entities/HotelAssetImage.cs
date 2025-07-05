using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class HotelAssetImage
    {
        public int Id { get; set; } 

        [Required][StringLength(255)] public string ImageId { get; set; }
        [Required][StringLength(500)] public string ImageURL { get; set; } 
        public string HotelAssetId { get; set; }
        public HotelAsset HotelAsset { get; set; }
    }
}
