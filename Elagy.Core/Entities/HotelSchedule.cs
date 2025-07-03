using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public  class HotelSchedule
    {
        public int Id { get; set; } 
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set;}
        public  DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
