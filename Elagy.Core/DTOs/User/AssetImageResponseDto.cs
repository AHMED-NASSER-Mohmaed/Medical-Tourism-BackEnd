using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class AssetImageResponseDto
    {
        public int Id { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
   
        public string AssetId { get; set; }
    }
}
