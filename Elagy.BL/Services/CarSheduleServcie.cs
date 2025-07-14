using Elagy.Core.DTOs.CarRentalSchedule;
using Elagy.Core.Entities;
using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Elagy.Core.DTOs.CarlSchedule;
using Microsoft.AspNetCore.Builder;

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
            await _unitOfWork.CompleteAsync();


            var query = _unitOfWork.CarSchedule.AsQueryable();

            query = query.Where(cs => cs.Id == carSchedule.Id);


            
            CarSchedule  createdCarSchedule  = await query.FirstOrDefaultAsync();

            var res= _map.Map<CarSheduleResponseDTO>(createdCarSchedule);

            res.price = car.PricePerDay;
            return res;
        }



        public async Task<bool> IsAvilable(DateOnly StartDate, DateOnly EndDate, int carId)
        {
            IQueryable<CarSchedule> query = _unitOfWork.CarSchedule.AsQueryable();

            query = query.Where(cs => (carId == cs.CarId) &&  ( (StartDate >= cs.StartingDate && StartDate <= cs.EndingDate) ||
                        (EndDate >= cs.StartingDate && EndDate <= cs.EndingDate) ));

            var result = await query.ToListAsync();

            return result.Count() == 0 ? true : false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////


        public async Task<CarUnavailableDatesDTO> GetAvailableCarsSchedules(int carId)
        {
            // 1. Ensure car exists and is available
            var car = await _unitOfWork.Cars.AsQueryable().Include(c => c.CarRentalAsset).
                FirstOrDefaultAsync(c => c.Id == carId && c.IsAvailable && c.Status != CarStatus.UnderMaintenance);

            if (car == null)
                throw new Exception("Car is not available or under maintenance.");

            var today = DateOnly.FromDateTime(DateTime.Today);

            // 2. Get all confirmed car schedules for this car
            var carSchedules = await _unitOfWork.CarSchedule.AsQueryable()
                .Where(cs => cs.CarId == carId &&
                             cs.Status == ScheduleStatus.Confirmed &&
                             cs.StartingDate >= today)
                .ToListAsync();

            /*var carAppointments = await _unitOfWork.CarRentalAppointments.AsQueryable()
                                   .Where(ca => ca.CarScheduleId == carId &&
                                                ca.Status != AppointmentStatus.Cancelled &&
                                                (ca.EndingDate) >= today)
                                   .ToListAsync();*/

            // 4. Collect all unavailable dates from both schedules and appointments
            List<Periode> unavailableDates = new List<Periode>();

            // From schedules
            foreach (var schedule in carSchedules)
            {
                /* for (var date = schedule.StartingDate; date <= schedule.EndingDate; date = date.AddDays(1))
                 {
                     unavailableDates.Add(date);
                 }*/

                unavailableDates.Add(new Periode { StartingDate = schedule.StartingDate, EndingDate= schedule.EndingDate });

            }

           /* // From appointments
            foreach (var appointment in carAppointments)
            {
                var start = appointment.StartingDate;
                var end = appointment.EndingDate;

                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    unavailableDates.Add(date);
                }
            }
*/
            // 5. Return DTO
            return new CarUnavailableDatesDTO
            {
                CarId = carId,
                CarModel = car.ModelName,
                CarRentalId = car.CarRentalAssetId,
                CarRentalName = car.CarRentalAsset.Name,
                UnavailableDates = unavailableDates
            };
        }
    }
}
