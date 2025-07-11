using AutoMapper;
using Elagy.Core.DTOs.CarRentalSchedule;
using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class CarSheduleServcie : ICarScheduleService
    {
 

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _map;

        public CarSheduleServcie(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _map = mapper;
        }
        /// <summary>
        /// /// Creates a new car schedule based on the provided DTO.
        /// /// This method checks if the car exists and then creates a new RoomSchedule entity.
        /// ///// If the car does not exist, it throws an ArgumentException.
        /// /// The method returns the created carSchedule entity.
        /// /// if the schdule overlap with another schedule it will throw an exception
        /// </summary>
        /// <param name="carScheduleDTO"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>

        public async Task<CarSheduleResponseDTO> CreateCarSchedule(CreateCarScheduleDTO carScheduleDTO)
        {
            Car car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carScheduleDTO.CarId);
            if (car == null)
            {
                throw new ArgumentException("Room not found.");
            }

            if (!await IsAvilable(carScheduleDTO.StartingDate, carScheduleDTO.EndingDate, car.Id))
            {
                throw new ArgumentException("The room is not available for the selected dates.");
            }


            CarSchedule carSchedule = _map.Map<CarSchedule>(carScheduleDTO);

           
            int numberOfDays = carScheduleDTO.EndingDate.DayNumber - carScheduleDTO.StartingDate.DayNumber + 1;
            carSchedule.TotalPrice = car.PricePerDay * numberOfDays;

            carSchedule.Car = car;

            carSchedule.CarId = car.Id;

            await _unitOfWork.CarSchedule.AddAsync(carSchedule);


            var query = _unitOfWork.CarSchedule.AsQueryable();



            query = query.Where(cs => cs.CarId == carScheduleDTO.CarId && cs.StartingDate == carScheduleDTO.StartingDate &&
                cs.EndingDate == cs.EndingDate);


            
            CarSchedule  createdCarSchedule  = await query.FirstOrDefaultAsync();

            return _map.Map<CarSheduleResponseDTO>(createdCarSchedule);
        }

      

        public async Task<bool> IsAvilable(DateOnly StartDate, DateOnly EndDate, int carId)
        {
            IQueryable<CarSchedule> query = _unitOfWork.CarSchedule.AsQueryable();

            query = query.Where(cs => (carId == cs.CarId) && (StartDate >= cs.StartingDate && StartDate <= cs.EndingDate) ||
                        (EndDate >= cs.StartingDate && EndDate <= cs.EndingDate));

            var result = await query.ToListAsync();

            return result.Count() != 0 ? true : false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<UnavailableDatesDTO> GetAvailableCarsSchedules(int carId)
        {
            var car =_unitOfWork.Cars.FindAsync(c=>c.Id== carId && c.IsAvailable && c.Status!=CarStatus.UnderMaintenance);

            if (car == null)
                throw new Exception("Car is not Available");

            var today = DateOnly.FromDateTime(DateTime.Today);

            var carschedules = _unitOfWork.CarSchedule.AsQueryable()
                .Where(cs => cs.CarId == carId && cs.Status == ScheduleStatus.Confirmed && cs.StartingDate>=today)
                .ToListAsync();

            var carAppointments =  _unitOfWork.CarRentalAppointments.AsQueryable()
       .Where(ca => ca.CarScheduleId == carId &&
                    ca.Status != AppointmentStatus.Cancelled &&
                    DateOnly.FromDateTime(ca.EndingDateTime) >= today)
       .ToListAsync();

            var unavailableDates=new HashSet<DateOnly>();


            throw new Exception("wait");


        }
    }
}
