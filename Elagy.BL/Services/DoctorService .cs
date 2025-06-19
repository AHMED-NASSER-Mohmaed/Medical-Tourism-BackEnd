using AutoMapper;
using Elagy.Core.DTOs.DoctorDTO;
using Elagy.Core.DTOs.SpecialtyDTO;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, ILogger<DoctorService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    // Admin Dashboard - Get all doctors for the table view
    public async Task<IEnumerable<DoctorTableDto>> GetAllDoctorsForAdminDashboardAsync(string hospitalId)
    {
        var doctors = await _unitOfWork.Doctors.GetAllDoctorsWithHospitalSpecialtyAndSpecialtyAsync(); // Gets all doctors with nav props
                                                                                                       // Filter by hospitalId in memory if the initial fetch is too broad, or add .Where() to repo method for efficiency
        var scopedDoctors = doctors.Where(d => d.HospitalSpecialty?.HospitalAssetId == hospitalId).ToList();

        List<DoctorTableDto> DOCDTOS = new List<DoctorTableDto>();
        foreach (var doctor in scopedDoctors)
        {
            DoctorTableDto DoctorDTO = new DoctorTableDto()
            {
                Id = doctor.Id,
                Name=doctor.FirstName+""+doctor.LastName,
                Email=doctor.Email,
                Specialist=doctor.HospitalSpecialty.Specialty.Name,
            };
            DOCDTOS.Add(DoctorDTO);

        }
        return DOCDTOS;
    }

    // Admin Dashboard - Get single doctor for "View Details" modal
    public async Task<DoctorTableDto> GetDoctorByIdForAdminAsync(string id, string hospitalId)
    {
        var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(id);

        // SCOPING LOGIC: Ensure the doctor belongs to this hospital
        if (doctor == null || doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
        {
            _logger.LogWarning($"Doctor ID {id} not found or does not belong to hospital ID {hospitalId}.");
            return null;
        }
        DoctorTableDto DoctorDTO = new DoctorTableDto()
        {
            Id = doctor.Id,
            Name = doctor.FirstName + "" + doctor.LastName,
            Email = doctor.Email,
            Specialist = doctor.HospitalSpecialty.Specialty.Name,
        };
        return DoctorDTO;
    }

    // Admin Dashboard - Create a new Doctor
    public async Task<DoctorTableDto> CreateDoctorAsync(DoctorCreateDto createDto, string hospitalId)
    {
        var hospitalSpecialty = await _unitOfWork.Hospitals.AsQueryable()
                                     .SelectMany(h => h.HospitalSpecialties)
                                     .Where(hs => hs.Id == createDto.HospitalSpecialtyId && hs.HospitalAssetId == hospitalId) // SCOPING VALIDATION
                                     .Include(hs => hs.Specialty) // Required for mapping DoctorDto later
                                     .Include(hs => hs.HospitalAsset) // Required for mapping DoctorDto later
                                     .FirstOrDefaultAsync();

        if (hospitalSpecialty == null)
        {
            _logger.LogWarning($"Attempted to create doctor with invalid or unauthorized HospitalSpecialtyId: {createDto.HospitalSpecialtyId} for hospital {hospitalId}");
            throw new ArgumentException($"The selected specialty/clinic (ID: {createDto.HospitalSpecialtyId}) is not valid or does not belong to your hospital.");
        }

        // 2. Check if email is already registered
        var userExists = await _userManager.FindByEmailAsync(createDto.Email);
        if (userExists != null)
        {
            _logger.LogWarning($"Attempted to create doctor with existing email: {createDto.Email}");
            throw new ArgumentException("Email address is already registered.");
        }

        Doctor doctor = new Doctor()
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            UserType = Elagy.Core.Enums.UserType.Doctor,
            Status = Elagy.Core.Enums.UserStatus.Active,
            EmailConfirmed = true,
            //NationalId = createDto.NationalId,
            //PassportId = createDto.PassportId,
            //Nationality=createDto.Nationality,
            //ImageId=createDto.ImageId,
            //ImageURL=createDto.ImageURL,
            Gender=createDto.Gender,
            //ZipCode=createDto.ZipCode,
            //StreetNumber=createDto.StreetNumber,
            Governorate=createDto.Governorate,
            DateOfBirth=createDto.DateOfBirth,
            YearsOfExperience=createDto.YearsOfExperience,
            Bio=createDto.Bio,
            Qualification=createDto.Qualification,
            MedicalLicenseNumber=createDto.MedicalLicenseNumber,
            HospitalSpecialtyId=createDto.HospitalSpecialtyId



        };

   
        // 5. Create the User via UserManager
        var result = await _userManager.CreateAsync(doctor, createDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError($"Doctor creation failed for {createDto.Email}: {errors}");
            throw new ApplicationException($"Failed to create doctor: {errors}");
        }

        // 6. Assign role
        await _userManager.AddToRoleAsync(doctor, "Doctor");

        _logger.LogInformation($"Doctor '{doctor.Email}' created successfully by admin for hospital ID: {hospitalId}. User ID: {doctor.Id}");

        // Manually set the navigation property for the DTO mapping if needed immediately, as Identity doesn't fill this
        doctor.HospitalSpecialty = hospitalSpecialty;

        return _mapper.Map<DoctorTableDto>(doctor);
    }

    // Admin Dashboard - Update existing Doctor
    public async Task<bool> UpdateDoctorAsync(DoctorUpdateDto updateDto, string hospitalId)
    {

        var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(updateDto.Id);
        if (doctor == null)
        {
            _logger.LogWarning($"Doctor with ID {updateDto.Id} not found for update.");
            return false;
        }

        // SCOPING LOGIC: Check if the doctor belongs to this hospital
        if (doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
        {
            _logger.LogWarning($"Hospital Admin {hospitalId} attempted to update doctor {updateDto.Id} which does not belong to their hospital.");
            throw new UnauthorizedAccessException("You do not have permission to update this doctor."); // Controller will catch this
        }

        // Validate HospitalSpecialtyId (the new one) exists AND belongs to the current hospital
        var newHospitalSpecialty = await _unitOfWork.Hospitals.AsQueryable()
                                        .SelectMany(h => h.HospitalSpecialties)
                                        .Where(hs => hs.Id == updateDto.HospitalSpecialtyId && hs.HospitalAssetId == hospitalId) // SCOPING VALIDATION
                                        .FirstOrDefaultAsync();

        if (newHospitalSpecialty == null)
        {
            _logger.LogWarning($"Attempted to update doctor {updateDto.Id} with invalid or unauthorized HospitalSpecialtyId: {updateDto.HospitalSpecialtyId} for hospital {hospitalId}");
            throw new ArgumentException($"The selected new specialty/clinic (ID: {updateDto.HospitalSpecialtyId}) is not valid or does not belong to your hospital.");
        }

        // Update IdentityUser properties
        doctor.Email = updateDto.Email;
        doctor.PhoneNumber = updateDto.PhoneNumber;
        doctor.FirstName = updateDto.FirstName;
        doctor.LastName = updateDto.LastName;
        //doctor.Nationality = updateDto.Nationality;
        //doctor.NationalId = updateDto.NationalId;
        //doctor.PassportId = updateDto.PassportId;
        doctor.ImageURL = updateDto.ImageURL;
        doctor.Gender = updateDto.Gender;
        doctor.Status = updateDto.Status;
        //doctor.ZipCode = updateDto.ZipCode;
        //doctor.StreetNumber = updateDto.StreetNumber;
        //doctor.Governorate = updateDto.Governorate;
        doctor.DateOfBirth = updateDto.DateOfBirth;

        var userResult = await _userManager.UpdateAsync(doctor);
        if (!userResult.Succeeded)
        {
            var errors = string.Join(", ", userResult.Errors.Select(e => e.Description));
            _logger.LogError($"Failed to update doctor user details for {updateDto.Id}: {errors}");
            throw new ApplicationException($"Failed to update doctor user details: {errors}");
        }

        // Update Doctor-specific properties, including the reassignment
        doctor.MedicalLicenseNumber = updateDto.MedicalLicenseNumber;
        doctor.YearsOfExperience = updateDto.YearsOfExperience;
        doctor.Bio = updateDto.Bio;
        doctor.Qualification = updateDto.Qualification;
        doctor.HospitalSpecialtyId = updateDto.HospitalSpecialtyId; // This is the REASSIGNMENT

        _unitOfWork.Doctors.Update(doctor); // Mark for update if not handled by UserManager update or if you add new properties outside Identity
        await _unitOfWork.CompleteAsync(); // Save specific doctor property changes
        _logger.LogInformation($"Doctor ID {updateDto.Id} profile updated successfully by hospital admin {hospitalId}.");
        return true;
    }

    // Admin Dashboard - Delete Doctor
    public async Task<bool> DeleteDoctorAsync(string id, string hospitalId) // Corrected parameter order
    {
        var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(id);
        if (doctor == null)
        {
            _logger.LogWarning($"Doctor with ID {id} not found for deletion.");
            return false; // Doctor not found at all
        }

        // --- SCOPING LOGIC ---
        // Critical step: Verify that the doctor to be deleted belongs to the current admin's hospital.
        if (doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
        {
            _logger.LogWarning($"Hospital Admin {hospitalId} attempted to delete doctor {id} which does not belong to their hospital.");
            // Throwing UnauthorizedAccessException is good practice for permission denial at the service layer.
            throw new UnauthorizedAccessException("You do not have permission to delete this doctor.");
        }

        // --- EXECUTE HARD DELETE ---
        // _userManager.DeleteAsync() performs a hard delete of the User record (and Doctor record if TPT cascade delete is configured).
        var result = await _userManager.DeleteAsync(doctor);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError($"Failed to delete doctor {id}: {errors}");
            // Throw ApplicationException for Identity-related errors during deletion
            throw new ApplicationException($"Failed to delete doctor: {errors}");
        }
        _logger.LogInformation($"Doctor ID {id} deleted successfully by hospital admin {hospitalId}.");
        return true; // Deletion successful
    }

    // Admin Helper: Get available HospitalSpecialties for assignment dropdown
    public async Task<IEnumerable<HospitalSpecialtyDto>> GetAvailableHospitalSpecialtiesForAssignmentAsync(string hospitalId)
    {
        var hospitalSpecialties = await _unitOfWork.Hospitals.GetHospitalSpecialtiesInHospitalAsync(hospitalId);
        var hospitalSpecialtyDtos = new List<HospitalSpecialtyDto>();

        foreach (var hsEntity in hospitalSpecialties)
        {
            var hsDto = new HospitalSpecialtyDto
            {
                Id = hsEntity.Id,
                HospitalAssetId = hsEntity.HospitalAssetId,
                SpecialtyId = hsEntity.SpecialtyId
            };

            // Explicitly map nested Specialty
            if (hsEntity.Specialty != null)
            {
                hsDto.Specialty = new SpecialtyDto
                {
                    Id = hsEntity.Specialty.Id,
                    Name = hsEntity.Specialty.Name,
                    Description = hsEntity.Specialty.Description
                };
            }

            // Explicitly map nested HospitalAsset (requires casting because HospitalAsset is derived from ServiceAsset)
            if (hsEntity.HospitalAsset != null)
            {
                var hospitalAsset = hsEntity.HospitalAsset as HospitalAsset; // Cast to HospitalAsset to access its specific properties
                if (hospitalAsset != null)
                {
                    hsDto.HospitalAsset = new HospitalMinDto
                    {
                        Id = hospitalAsset.Id,
                        //Name = hospitalAsset.AssetName, // AssetName from ServiceAsset base
                     
                    };
                }
            }
            hospitalSpecialtyDtos.Add(hsDto);
        }

        return hospitalSpecialtyDtos;
    }
}

   

