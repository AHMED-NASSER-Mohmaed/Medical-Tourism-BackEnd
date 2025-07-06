using Elagy.Core.DTOs.RoomAppoinment;
using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class RoomAppointmentService : IRoomAppointmentService
    {
        private readonly IRoomScheduleService _roomScheduleService;
        private readonly IUnitOfWork _unitOfWork;

        public RoomAppointmentService(IRoomScheduleService roomScheduleService,IUnitOfWork unitOfWork)
        {
            _roomScheduleService = roomScheduleService;
            _unitOfWork = unitOfWork;
        }
    
        public async Task<Package> BookAppointment(Package createdPacakge, RoomAppointmentResponseDTO radto)
        {

            try
            {
                CreateRoomScheduleResponseDTO rs = await _roomScheduleService.CreateRoomSchedule(new CreateRoomScheduleDTO
                {
                    StartDate=radto.CheckInDate,
                    EndDate = radto.CheckOutDate,
                    RoomId=radto.RoomId
                });

                createdPacakge.Appointments.Add(new RoomAppointment
                {
                    price = rs.Price,
                    Type = AppointmentType.Car,
                    Status = AppointmentStatus.Booked,
                    PackageId = createdPacakge.Id,
                    CheckInDate = rs.StartDate,
                    CheckOutDate = rs.EndDate,
                    HotelScheduleId = rs.Id,
                    RoomId = rs.RoomId,
                });

                _unitOfWork.Packages.Update(createdPacakge);

                return createdPacakge;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while booking the car appointment.", ex);
            }


         }
    }
}
