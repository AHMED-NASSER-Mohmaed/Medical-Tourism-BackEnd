using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class CarAppointment:Appointment
    {
        DateOnly StartingDate { get; set; }
        DateOnly EndingDate { get; set; }


        string ServiceProviderId { get; set; }
        ServiceProvider ServiceProvider { get; set; }
    }
}
