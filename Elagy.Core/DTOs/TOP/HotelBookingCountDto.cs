using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.TOP
{
    public class HotelBookingCountDto
    {
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelDescription{ get; set; }
        public int? starRating { get; set; }

        public int BookingCount { get; set; }
    }

}
