using AutoMapper;
using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class RoomScheduleService : IRoomScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _map;

        public RoomScheduleService(IUnitOfWork unitOfWork,IMapper map)
        {
            _unitOfWork = unitOfWork;
            _map = map;
        }
        /// <summary>
        /// /// Creates a new room schedule based on the provided DTO.
        /// /// This method checks if the room exists and then creates a new RoomSchedule entity.
        /// ///// If the room does not exist, it throws an ArgumentException.
        /// /// The method returns the created RoomSchedule entity.
        /// /// if the schdule overlap with another schedule it will throw an exception
        /// </summary>
        /// <param name="roomScheduleDTO"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>

        public async Task<CreateRoomScheduleResponseDTO> CreateRoomSchedule(CreateRoomScheduleDTO roomScheduleDTO)
        {
            Room room=await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomScheduleDTO.RoomId);
            if (room == null)
            {
                throw new ArgumentException("Room not found.");
            }

            if(! await IsAvilable(roomScheduleDTO.StartDate, roomScheduleDTO.EndDate,room.Id))
            {
                throw new ArgumentException("The room is not available for the selected dates.");
            }


            RoomSchedule roomSchedule = _map.Map<RoomSchedule>(roomScheduleDTO);

            int numberOfDays = roomScheduleDTO.EndDate.DayNumber - roomScheduleDTO.StartDate.DayNumber + 1;
            roomSchedule.TotalPrice = room.Price * numberOfDays;

            await _unitOfWork.RoomSchedule.AddAsync(roomSchedule);


            var query = _unitOfWork.RoomSchedule.AsQueryable();

            
            query=query.Where(rs=>rs.RoomId==roomScheduleDTO.RoomId && rs.StartDate==roomScheduleDTO.StartDate &&
                rs.EndDate==roomScheduleDTO.EndDate);

           RoomSchedule  createdRoomSchedule =await query.FirstOrDefaultAsync();

            return _map.Map<CreateRoomScheduleResponseDTO>(createdRoomSchedule);
        }

        public async Task<bool> IsAvilable(DateOnly StartDate, DateOnly EndDate , int roomId)
        {
            IQueryable<RoomSchedule>  query = _unitOfWork.RoomSchedule.AsQueryable();

            query= query.Where(rs => (roomId == rs.RoomId)&& (StartDate >= rs.StartDate && StartDate<=rs.EndDate )||
                        (EndDate >= rs.StartDate && EndDate <= rs.EndDate));

            var result = await query.ToListAsync();

            return result.Count()!=0?true:false;
        }

        
    }
}
