using Elagy.Core.DTOs.Booking;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class BookingService : IBookingService
    {
        private readonly ISpecialtyScheduleService _scheduleService;

        public BookingService(ISpecialtyScheduleService scheduleService)
        {
            _scheduleService= scheduleService;
        }
        public async Task<Package> CreatePendingBookingAsync(CreateBookingRequest request)
        {
            //fetch scheduled appointments from the request

            ScheduleResponseDto schedule = await _scheduleService.GetScheduleByIdAsync(request.HospitalAppointment.HotelScheduleId);

            if (schedule == null)
            {
                throw new ArgumentException("Invalid schedule ID provided in the booking request.");
            }

            if(request.HotelAppointment != null)
            {
            }

            if (!schedule.IsActive)
            {
                throw new InvalidOperationException("The schedule is not active and cannot be booked.");
            }

            if (schedule.AvailableSlots <= 0)
            {
                throw new InvalidOperationException("No available slots for the selected schedule.");
            }

            return new Package();
        }

        public Task<Package?> GetBookingByIdAsync(int PackageId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookingStatusAsync(int PackageId, BookingStatus newStatus, string? stripeSessionId = null, string? stripePaymentIntentId = null)
        {
            throw new NotImplementedException();
        }
    }
}
