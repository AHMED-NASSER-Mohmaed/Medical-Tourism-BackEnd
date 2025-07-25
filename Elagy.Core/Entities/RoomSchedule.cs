﻿using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public  class RoomSchedule
    {
        public int Id { get; set; } 
        public decimal TotalPrice { get; set; }
        public  DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        //public ScheduleStatus RoomscheduleStatus { get; set; } = ScheduleStatus.Pending;
        public ScheduleStatus RoomscheduleStatus { get; set; } = ScheduleStatus.Confirmed;
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
