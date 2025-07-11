using Elagy.Core.DTOs.RoomAppoinment;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IRoomAppointmentService
    {

        Task<Package> BookAppointment(Package createdPackage, CreateRoomAppointmentDTO rdto);

    }
}
