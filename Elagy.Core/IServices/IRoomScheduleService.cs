using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.DTOs.TOP;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IRoomScheduleService
    {
        Task<bool> IsAvilable(DateOnly Start, DateOnly End,int roomId);
        Task<CreateRoomScheduleResponseDTO> CreateRoomSchedule(CreateRoomScheduleDTO roomScheduleDTO);
        //-------------------------------------------------------------------------------------
        Task<UnavailableDatesDTO> GetAvailableRoomsSchedules(int RoomId);
    }
}
