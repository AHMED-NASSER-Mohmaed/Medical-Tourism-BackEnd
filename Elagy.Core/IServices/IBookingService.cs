using Elagy.Core.DTOs.Booking;
using Elagy.Core.DTOs.Package;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IBookingService
    {

        Task<PackageResponseDTO> CreatePendingBookingAsync(string PatientId,CreateBookingRequest request);
        Task<Package?> GetBookingByIdAsync(int PackageId);
        Task UpdateBookingStatusAsync(int PackageId, BookingStatus newStatus, string? stripeSessionId = null, string? stripePaymentIntentId = null);

    }

}
