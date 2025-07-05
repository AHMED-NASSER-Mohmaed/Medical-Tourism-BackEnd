using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class HotelProviderProfileUpdateDto : BaseServiceProviderProfileUpdateDto
    {
        public int? StarRating { get; set; }
        public bool? HasPool { get; set; }
        public bool? HasRestaurant { get; set; }
    }
}
