using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum CarStatus // Current status of a car
    {
        Available = 0,      // Ready for rent
        OnRide = 1,         // Currently rented out
        UnderMaintenance = 2, // Out of service for repairs
        Unavailable = 3     // Permanently out of service or decommissioned
    }
}
