using AutoMapper;
using Elagy.Core.DTOs.Booking;
using Elagy.Core.DTOs.Package;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class BookingService : IBookingService
    {
        private readonly ISpecialtyAppointmentService _specialtyAppointment;
        private readonly IRoomAppointmentService _roomApointmentService;
        private readonly ICarAppointmentService _carAppointmentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(ISpecialtyAppointmentService specialtyAppointment,IMapper mapper,IUnitOfWork unitOfWork, IRoomAppointmentService roomApointmentService)
        {
            _specialtyAppointment = specialtyAppointment;
            _roomApointmentService = roomApointmentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<PackageResponseDTO> CreatePendingBookingAsync(string PatientId , CreateBookingRequest request)
        {
             
           if (request == null)
           {
               throw new ArgumentNullException(nameof(request), "Booking request cannot be null.");
           }
           if (string.IsNullOrEmpty(PatientId))
           {
               throw new ArgumentException("Patient ID cannot be null or empty.", nameof(PatientId));
           }

            try
            {


                if(request.SpecialtiyAppointment == null)
                {
                    throw new ArgumentNullException(nameof(request.SpecialtiyAppointment), "Specialty appointment cannot be null.");
                }

                Package CreatedPackage= await _specialtyAppointment.BookAppointment(PatientId, request.SpecialtiyAppointment);

                SpecialtyAppointmentResponseDTTO _specialtyResponseDto = _mapper.Map<SpecialtyAppointmentResponseDTTO>(CreatedPackage.Appointments[0]);


                PackageResponseDTO _packageResponseDTO = _mapper.Map<PackageResponseDTO>(CreatedPackage);

                _packageResponseDTO.SpecialtyAppoinment=_specialtyResponseDto;


                if (request.RoomAppointment!=null)
                    await _roomApointmentService.BookAppointment(CreatedPackage, request.RoomAppointment);

                if(request.CarAppointment != null)
                    await _carAppointmentService.BookAppointment(CreatedPackage, request.CarAppointment);

                await _unitOfWork.CompleteAsync();

                return _packageResponseDTO;

            }
            catch(Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while creating the booking.", ex);
            }


        }

        public Task UpdateBookingStatusAsync(int PackageId, BookingStatus newStatus, string? stripeSessionId = null, string? stripePaymentIntentId = null)
        {
            throw new NotImplementedException();
        }
    }
}
