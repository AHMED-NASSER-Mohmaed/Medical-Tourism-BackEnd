using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class AssetImageCreateDto
    {
        [Required][StringLength(255)] public string ImageId { get; set; }
        [Required][StringLength(500)] public string ImageURL { get; set; }
    }
}
