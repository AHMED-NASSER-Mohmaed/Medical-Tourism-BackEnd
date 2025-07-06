using Elagy.Core.DTOs.SpecialtyAppointment;
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
    public class SpecialtyAppointmentServcie : ISpecialtyAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPackgeService _packgeService;
        private readonly IRoomScheduleService _RoomScheduleService;
        public SpecialtyAppointmentServcie(IUnitOfWork unitOfWork,IPackgeService packgeService, IRoomScheduleService roomScheduleService)
        {
            _unitOfWork = unitOfWork;
            _packgeService = packgeService;
            _RoomScheduleService = roomScheduleService;
        }

        public async Task<Package> BookAppointment(string patientId, CreateSpecialtyAppointmentDTO SADTO)
        {
            if (SADTO.SpecialtyScheduleId <= 0)
            {
                throw new ArgumentException("Invalid Specialty Schedule ID", nameof(SADTO.SpecialtyScheduleId));
            }

            (bool isAvailable, int nOfBookedAppoinment ,SpecialtySchedule ss) = await IsAvailableAppointmentForBooking(SADTO.AppointmentDate, SADTO.SpecialtyScheduleId);

            if (!isAvailable)
            {
                throw new InvalidOperationException("No available appointments for booking on the selected date.");
            }

            try
            {

                //Package createPackage = await _packgeService.CreatePackage(patientId);

                double TotalMinutes = ( (ss.TimeSlotSize.Hours * 60) + ss.TimeSlotSize.Minutes ) * nOfBookedAppoinment;


                /* createPackage.Appointments.Add(new SpecialtyAppointment
                 {
                     price = ss.Price,
                     Type = AppointmentType.Specialty,
                     //Status = AppointmentStatus.Pending,
                     Status = AppointmentStatus.Booked,
                     PackageId = createPackage.Id,
                     Package = createPackage,
                     ServiceDeliveryType = SpecialtyAppointmentDeliveryType.Onsite, // Assuming on-site delivery for specialty appointments

                     Date = SADTO.AppointmentDate,

                     ExistingTime = ss.StartTime.AddMinutes(TotalMinutes),

                     SpecialtyScheduleId = SADTO.SpecialtyScheduleId,

                 });


                 _unitOfWork.Packages.Update(createPackage);
 */

                //CreateRoomSchedule();

                 

                Guid packageId = Guid.NewGuid(); // Generate a new package ID

                var createPackage = new Package
                {
                    Id = packageId,
                    PatientId = patientId,
                    CreatedAt = DateTime.UtcNow,
                    Status = BookingStatus.Confirmed,
                    Appointments = new List<Appointment>
                    {
                        new SpecialtyAppointment
                        {
                            price = ss.Price,
                            Type = AppointmentType.Specialty,
                            Status = AppointmentStatus.Booked,
                            PackageId = packageId,
                            ServiceDeliveryType = SpecialtyAppointmentDeliveryType.Onsite,
                            Date = SADTO.AppointmentDate,
                            ExistingTime = ss.StartTime.AddMinutes(TotalMinutes),
                            SpecialtyScheduleId = SADTO.SpecialtyScheduleId
                        }
                    }
                };

                await _unitOfWork.Packages.AddAsync(createPackage);

                return createPackage;

            }
            catch (Exception ex)
            {
                // Handle exceptions, log them, or rethrow as needed
                throw new InvalidOperationException("An error occurred while booking the specialty appointment.", ex);
            }
        }

        public async Task<(int booked, int cancelled)> GetBookedAndCancelled(int _scheduleId)
        {

            IQueryable<SpecialtyAppointment> QueryCancelledAppoinment = _unitOfWork.SpecialtyAppointments.AsQueryable();
            QueryCancelledAppoinment = QueryCancelledAppoinment.Where(a => a.SpecialtyScheduleId == _scheduleId && a.Status == AppointmentStatus.Cancelled);

            IQueryable<SpecialtyAppointment> QueryBookedAppoinment = _unitOfWork.SpecialtyAppointments.AsQueryable();
            QueryBookedAppoinment = QueryBookedAppoinment.Where(a => a.SpecialtyScheduleId == _scheduleId && a.Status == AppointmentStatus.Booked);

            return (await QueryBookedAppoinment.CountAsync(), await QueryCancelledAppoinment.CountAsync());
        }

        public async Task<(bool IsAvailable, int AppointmentCount, SpecialtySchedule SS)> IsAvailableAppointmentForBooking(DateOnly Date, int SpecialtyScheduleId)
        {

            //fetch Schedule and check if this date maches the schedule day

            SpecialtySchedule SS =await _unitOfWork.SpecialtySchedule.GetScheduleByIdWithDetailsAsync(SpecialtyScheduleId);

            if (SS == null)
            {
                throw new ArgumentException("Specialty Schedule not found", nameof(SpecialtyScheduleId));
            }

            int DayId = (int)Date.DayOfWeek+1; // Convert DateOnly to DayOfWeek enum

            if (DayId!=SS.DayOfWeekId)
            {
                throw new ArgumentException("Invalid Date of Speciality Schdule : the selectDate not mathc any Speciality Schdule");
            }

            // fetch all confrimed appointments for this schedule at this day


            var query= _unitOfWork.SpecialtyAppointments.AsQueryable(); 

            query = query.Where(a => a.SpecialtyScheduleId == SpecialtyScheduleId && 
                            a.Date == Date && (a.Status != AppointmentStatus.Cancelled ));

            var count = await query.CountAsync();

            //them compare count to max capacity of schedule

            return (count < SS.MaxCapacity , count ,SS);
        }
 
    }
}
