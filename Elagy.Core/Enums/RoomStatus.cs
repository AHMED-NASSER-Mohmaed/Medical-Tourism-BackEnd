using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum RoomStatus 
    {
        CleanAndAvailable = 0,
        Occupied = 1, // client already exist in room
        UnderMaintenance = 2,
        Reserved = 3 // client reserved it and waiting the date
    }
}
