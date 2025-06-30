using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity; // For UserManager in Admin Add
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IEmailService = Elagy.Core.Helpers.IEmailService;

namespace Elagy.BL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;
        private readonly UserManager<User> _userManager; // Needed for AdminAdd
        private readonly IEmailService _emailService; // Needed for AdminAdd

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger,
                              UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<PatientDto> GetPatientProfileAsync(string patientId)
        {
            // Use the correct overload for including navigation properties in EF Core
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, new Expression<Func<Patient, object>>[] { p => p.Governorate , p => p.Governorate.Country});

            if (patient == null)
            {
                _logger.LogWarning($"Patient with ID {patientId} not found.");
                return null;
            }
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> UpdatePatientProfileAsync(string patientId, PatientProfileUpdateDto model)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning($"Failed to update: Patient with ID {patientId} not found.");
                return null;
            }

            _mapper.Map(model, patient); // Map updated properties to the entity

            _unitOfWork.Patients.Update(patient); // Mark for update
            await _unitOfWork.CompleteAsync(); // Save changes

            _logger.LogInformation($"Patient with ID {patientId} profile updated successfully.");
            return _mapper.Map<PatientDto>(patient); // Return updated DTO
        }

        public async Task<AuthResultDto> AddPatientByAdminAsync(PatientRegistrationRequestDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

            var patient = _mapper.Map<Patient>(model);
            patient.UserType = UserType.Patient;
            patient.Status = Status.EmailUnconfirmed; // Still requires email confirmation
            patient.EmailConfirmed = false; // Explicitly set to false

            var result = await _userManager.CreateAsync(patient, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError($"Admin Patient creation failed for {patient.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            // Send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(patient);
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-email?userId={patient.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(patient.Email, "Confirm Your Email - Admin Added Account", $"Your account was created by an administrator. Please confirm your email to activate it: <a href='{confirmationLink}'>link</a>");

            _logger.LogInformation($"Patient {patient.Email} added by admin. Confirmation email sent.");
            return new AuthResultDto { Success = true, Message = "Patient account created successfully by admin. Email confirmation required for activation." };
        }
    }
}