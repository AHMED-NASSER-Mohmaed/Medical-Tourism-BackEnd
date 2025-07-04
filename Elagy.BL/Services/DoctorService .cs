
using AutoMapper;
using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities; 
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories; 
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;
using System;

namespace Elagy.BL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<DoctorService> _logger;
        private readonly UserManager<User> _userManager;

        public DoctorService(IFileStorageService fileStorageService, IUnitOfWork unitOfWork, IMapper mapper, ILogger<DoctorService> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _fileStorageService = fileStorageService;

        }

        /// Retrieves a paginated list of doctors for the Hospital Admin Dashboard,
        /// filtered by the hospital ID and optional search/status parameters.
        public async Task<DoctorProfileDto?> GetDoctorByIdAsync(string doctorId)
        {
            try
            {
 
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogInformation($"Doctor with ID {doctorId} not found in repository.");
                    return null; // Return null if the doctor is not found
                }
                return _mapper.Map<DoctorProfileDto>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving doctor by ID {doctorId}.");
                throw; 
            }
        }


        public async Task<PagedResponseDto<DoctorProfileDto>> GetDoctorsBySpecialtyIdForAdminDashboardAsync(
    int specialtyId, PaginationParameters paginationParameters)
        {
            try
            {
                // 1. Get all doctors for this SpecialtyId (includes full navigation via .Include())
                var doctors = await _unitOfWork.Doctors.GetDoctorsBySpecialtyIdAsync(specialtyId);

                // 2. Convert to queryable for in-memory filtering
                var query = doctors.AsQueryable();

                // 3. Apply SearchTerm filter
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(term) ||
                        d.LastName.ToLower().Contains(term) ||
                        d.Email.ToLower().Contains(term)
                    );
                }


                // 6. Pagination
                var totalCount = query.Count();
                var pagedDoctors = query
                    .OrderBy(d => d.FirstName)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                // 7. Mapping
                var doctorDtos = _mapper.Map<IEnumerable<DoctorProfileDto>>(pagedDoctors);

                return new PagedResponseDto<DoctorProfileDto>(
                    doctorDtos,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving doctors for Specialty ID: {specialtyId}");
                return new PagedResponseDto<DoctorProfileDto>(
                    Enumerable.Empty<DoctorProfileDto>(),
                    0,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );
            }
        }
        public async Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsForAdminDashboardAsync(string hospitalId, PaginationParameters paginationParameters)
        {
            try
            {
                // 1. Get all doctors affiliated with the specific hospital.
                var doctors = await _unitOfWork.Doctors.GetDoctorsByHospitalIdAsync(hospitalId);

                // 2. Apply search term filter in service (on the in-memory or already-queried set).

                var query = doctors.AsQueryable();
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(term) ||
                        d.LastName.ToLower().Contains(term) ||
                        d.Email.ToLower().Contains(term) 
                       
                    );
                }

                // 3. Apply UserStatus filter (Active, Deactivated, PendingApproval)
   
                if (paginationParameters.UserStatus.HasValue)
                {
                    query = query.Where(d => d.Status == paginationParameters.UserStatus.Value);
                }

                if(paginationParameters.SpecialtyId.HasValue)
                {
                    query = query.Where(s => s.HospitalSpecialty.SpecialtyId == paginationParameters.SpecialtyId);
                }
                var totalCount = query.Count();

                // 5. Apply pagination (Skip and Take) for the current page.
                var pagedDoctors = query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                // 6. Map the filtered and paginated Doctor entities to DoctorProfileDto.
                var doctorDtos = _mapper.Map<IEnumerable<DoctorProfileDto>>(pagedDoctors);

                // 7. Construct and return the PagedResponseDto.
                return new PagedResponseDto<DoctorProfileDto>(doctorDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctors for admin dashboard for Hospital ID: {hospitalId}.");
                // Return an empty paginated response on error, to avoid breaking the caller.
                return new PagedResponseDto<DoctorProfileDto>(Enumerable.Empty<DoctorProfileDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }

        /// Creates a new doctor account, assigning them to a specific hospital specialty.
      
        public async Task<DoctorProfileDto> CreateDoctorAsync(DoctorCreateDto createDto, string hospitalId, IFormFile? licenseDocumentFile, IFormFile? profileImageFile)
        {
            try
            {
                // 1. Validate HospitalSpecialtyId and its affiliation with the hospitalId
                // GetByIdWithDetailsAsync is assumed to load related Specialty and HospitalAsset for validation.
                var hospitalSpecialty = await _unitOfWork.HospitalSpecialties.GetByIdWithDetailsAsync(createDto.HospitalSpecialtyId);
                if (hospitalSpecialty == null)
                {
                    _logger.LogWarning($"Doctor creation failed: HospitalSpecialty with ID {createDto.HospitalSpecialtyId} not found.");
                    throw new ArgumentException($"Hospital Specialty with ID {createDto.HospitalSpecialtyId} does not exist.");
                }
                // Ensure the selected HospitalSpecialty truly belongs to the hospital of the current admin.
                if (hospitalSpecialty.HospitalAssetId != hospitalId)
                {
                    _logger.LogWarning($"Doctor creation failed: HospitalSpecialty {createDto.HospitalSpecialtyId} does not belong to Hospital {hospitalId}.");
                    throw new InvalidOperationException($"Hospital Specialty with ID {createDto.HospitalSpecialtyId} does not belong to your hospital.");
                }
                // Check if the global Specialty itself is active. A doctor shouldn't be assigned to an inactive global specialty.
                if (!hospitalSpecialty.Specialty.IsActive)
                {
                    _logger.LogWarning($"Doctor creation failed: Associated global specialty '{hospitalSpecialty.Specialty.Name}' is inactive.");
                    throw new InvalidOperationException($"Cannot create doctor for an inactive global specialty.");
                }

                //2.Validate Governorate and Country IDs for the doctor's address.

                string licenseUrl = null;
                string licenseId = null;
                if (licenseDocumentFile != null)
                {
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(licenseDocumentFile);
                    if (uploadResult.Success) { licenseUrl = uploadResult.Url; licenseId = uploadResult.Id; }
                    else { throw new InvalidOperationException($"Failed to upload doctor license document: {uploadResult.Message}"); }
                }
                else { throw new ArgumentException("Doctor license document is required."); }

                // 2. Handle Profile Image Upload
                string imageUrl = null;
                string imageId = null;
                if (profileImageFile != null)
                {
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(profileImageFile);
                    if (uploadResult.Success) { imageUrl = uploadResult.Url; imageId = uploadResult.Id; }
                    else { _logger.LogWarning($"Failed to upload profile image for {createDto.Email}. Proceeding without image. Details: {uploadResult.Message}"); }
                }


                else { throw new ArgumentException("Doctor license document is required."); } // License must be provided


                var governorate = await _unitOfWork.Governates.GetByIdAsync( createDto.GovernorateId);
                if (governorate == null)
                    {
                        _logger.LogWarning($"Doctor creation failed: Governorate with ID {createDto.GovernorateId} not found.");
                        throw new ArgumentException($"Governorate with ID {createDto.GovernorateId} does not exist.");
                    }
                // Assuming Governorate entity has a CountryId property to validate consistency.
                if (governorate.CountryId != createDto.CountryId)
                {
                    _logger.LogWarning($"Doctor creation failed: Country ID {createDto.CountryId} does not match Governorate {createDto.GovernorateId}.");
                    throw new ArgumentException($"Country with ID {createDto.CountryId} does not match the selected Governorate.");
                }

                // 3. Check if the provided email is already in use by another Identity user.
                if (await _userManager.FindByEmailAsync(createDto.Email) != null)
                {
                    _logger.LogWarning($"Doctor creation failed: Email '{createDto.Email}' is already registered.");
                    throw new InvalidOperationException($"Email '{createDto.Email}' is already registered.");
                }

                // 4. Map DTO to Doctor entity (which inherits from IdentityUser).


                var doctor = _mapper.Map<Doctor>(createDto);
                doctor.PhoneNumber = createDto.Phone;
                doctor.MedicalLicenseNumberURL = licenseUrl; // Set from upload result
                doctor.MedicalLicenseNumberId = licenseId;   // Set from upload result
                doctor.ImageURL = imageUrl; // Set profile image URL (from User base)
                doctor.ImageId = imageId;   // Set profile image ID (from User base)
                doctor.HospitalSpecialtyId = createDto.HospitalSpecialtyId;
                doctor.Status = Status.Active;
                doctor.UserType = UserType.Doctor;
                doctor.EmailConfirmed = true;
                doctor.PhoneNumberConfirmed = true;
                doctor.TwoFactorEnabled = true;
                doctor.LockoutEnabled = true;
                doctor.AccessFailedCount = 5;
                doctor.PhoneNumber = createDto.Phone; 

                // 5. Create the user with IdentityUserManager.
                var result = await _userManager.CreateAsync(doctor, createDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to create Identity user for doctor: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    throw new InvalidOperationException($"Failed to create doctor account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // 6. Assign the "Doctor" role to the newly created user.
                await _userManager.AddToRoleAsync(doctor, "Doctor"); // Assumes "Doctor" role exists in your Identity system.

                // 7. Complete Unit of Work.
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Doctor '{doctor.FirstName} {doctor.LastName}' (ID: {doctor.Id}) created successfully for Hospital {hospitalId}.");

                // 8. Re-fetch the newly created doctor with all details for the comprehensive DoctorProfileDto.

                var createdDoctorWithDetails = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(doctor.Id);
                if (createdDoctorWithDetails == null)
                {
                    _logger.LogError($"Created doctor {doctor.Id} not found immediately after creation for detail fetch.");
                    throw new Exception("Created doctor not found after successful creation."); // This is an unexpected internal error.
                }

                return _mapper.Map<DoctorProfileDto>(createdDoctorWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating doctor for Hospital {hospitalId}.");
                throw; // Re-throw the exception to the controller for appropriate HTTP response handling.
            }
        }


        public async Task<DoctorProfileDto> UpdateDoctorAsync(string doctorId, DoctorUpdateDto updateDto, string hospitalId, IFormFile? newLicenseDocumentFile = null, IFormFile? newProfileImageFile = null)
        {
            try
            {
              
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning($"Doctor with ID {doctorId} not found for update.");
                    throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
                }

                // 2. Validate doctor's affiliation to the requesting hospital admin.
                // Ensures that the admin can only update doctors belonging to their hospital.
                if (doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
                {
                    _logger.LogWarning($"Hospital Admin {hospitalId} attempted to update doctor {doctorId} not affiliated with their hospital.");
                    throw new UnauthorizedAccessException($"Doctor with ID {doctorId} is not affiliated with your hospital.");
                }


                if (newLicenseDocumentFile != null)
                {
                    if (!string.IsNullOrEmpty(doctor.MedicalLicenseNumberId)) { await _fileStorageService.DeleteFileAsync(doctor.MedicalLicenseNumberId); }
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(newLicenseDocumentFile);
                    if (uploadResult.Success) { doctor.MedicalLicenseNumberURL = uploadResult.Url; doctor.MedicalLicenseNumberId = uploadResult.Id; }
                    else { throw new InvalidOperationException($"Failed to upload new license document: {uploadResult.Message}"); }
                }

                // 2. Handle Profile Image Update
                if (newProfileImageFile != null)
                {
                    if (!string.IsNullOrEmpty(doctor.ImageId)) { await _fileStorageService.DeleteFileAsync(doctor.ImageId); }
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(newProfileImageFile);
                    if (uploadResult.Success) { doctor.ImageURL = uploadResult.Url; doctor.ImageId = uploadResult.Id; }
                    else { _logger.LogWarning($"Failed to upload new profile image. Proceeding without image. Details: {uploadResult.Message}"); }
                }



                // 3. Validate and update HospitalSpecialtyId if it's being changed.
                if (doctor.HospitalSpecialtyId != updateDto.HospitalSpecialtyId)
                {
                    var newHospitalSpecialty = await _unitOfWork.HospitalSpecialties.GetByIdWithDetailsAsync(updateDto.HospitalSpecialtyId);
                    if (newHospitalSpecialty == null)
                    {
                        throw new ArgumentException($"New Hospital Specialty with ID {updateDto.HospitalSpecialtyId} does not exist.");
                    }
                    // Ensure the new HospitalSpecialty also belongs to the same hospital as the admin.
                    if (newHospitalSpecialty.HospitalAssetId != hospitalId)
                    {
                        throw new InvalidOperationException($"New Hospital Specialty with ID {updateDto.HospitalSpecialtyId} does not belong to your hospital.");
                    }
                    // Ensure the global Specialty itself is active. Doctors should only be assigned to active specialties.
                    if (!newHospitalSpecialty.Specialty.IsActive)
                    {
                        throw new InvalidOperationException($"Cannot assign doctor to an inactive global specialty.");
                    }
                    doctor.HospitalSpecialtyId = updateDto.HospitalSpecialtyId; // Update FK on the doctor entity.
                }

            
                var governorate = await _unitOfWork.Governates.GetByIdAsync(updateDto.GovernorateId);
                if (doctor.GovernorateId != updateDto.GovernorateId || doctor.Governorate.CountryId != updateDto.CountryId)
                {
                    //var governorate = await _unitOfWork.Governorates.GetByIdAsync(updateDto.GovernorateId);
                    if (governorate == null) throw new ArgumentException($"Governorate with ID {updateDto.GovernorateId} does not exist.");
                    // Ensure the selected CountryId is consistent with the Governorate's CountryId.
                    if (governorate.CountryId != updateDto.CountryId) throw new ArgumentException($"Country with ID {updateDto.CountryId} does not match the selected Governorate.");
                }
                // Update GovernorateId and CountryId on the doctor entity.
                doctor.GovernorateId = updateDto.GovernorateId;
                doctor.Governorate.CountryId = updateDto.CountryId; // Update CountryId directly on the doctor entity (User base).

                // 5. Update user-level properties managed by Identity (Email, PhoneNumber, Status)
                if (!string.Equals(doctor.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUserWithNewEmail = await _userManager.FindByEmailAsync(updateDto.Email);
                    if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != doctor.Id)
                    {
                        throw new InvalidOperationException($"Email '{updateDto.Email}' is already registered to another user.");
                    }
                    // Directly updating Email and UserName (UserName often mirrors Email in Identity).
                    // This is acceptable for admin-initiated updates, but for user-initiated, ChangeEmailAsync with token is more secure.
                    doctor.Email = updateDto.Email;
                    doctor.UserName = updateDto.Email; // Keep UserName consistent with Email.
                }

                if (newLicenseDocumentFile != null)
                {
                    if (!string.IsNullOrEmpty(doctor.MedicalLicenseNumberId)) // Delete old file from storage if exists
                    {
                        var deleteResult = await _fileStorageService.DeleteFileAsync(doctor.MedicalLicenseNumberId);
                        if (!deleteResult) _logger.LogWarning($"Failed to delete old license document {doctor.MedicalLicenseNumberId} for doctor {doctorId}.");
                    }
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(newLicenseDocumentFile, $"doctors/licenses/{doctor.Email.Replace("@", "_").Replace(".", "_")}");
                    if (uploadResult.Success) { doctor.MedicalLicenseNumberURL = uploadResult.Url; doctor.MedicalLicenseNumberId = uploadResult.Id; }
                    else { throw new InvalidOperationException($"Failed to upload new license document: {uploadResult.Message}"); }
                }
               
                doctor.PhoneNumber = updateDto.PhoneNumber; // Update phone number.
                doctor.Status = updateDto.Status; // Update doctor's overall status (e.g., Active, Deactivated).

                // Update the Identity user's profile and save changes.
                var userUpdateResult = await _userManager.UpdateAsync(doctor);
                if (!userUpdateResult.Succeeded)
                {
                    _logger.LogError($"Failed to update Identity user for doctor {doctorId}: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                    throw new InvalidOperationException($"Failed to update doctor account: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                }

                // 6. Map remaining DTO properties to the entity for doctor-specific fields.
                // This maps FirstName, LastName, Gender, Address, City, DateOfBirth, MedicalLicenseNumber, YearsOfExperience, Bio, Qualification.
                _mapper.Map(updateDto, doctor);

 
                _unitOfWork.Doctors.Update(doctor); 
                await _unitOfWork.CompleteAsync(); 


                var updatedDoctorWithDetails = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(doctorId);
                if (updatedDoctorWithDetails == null) throw new Exception("Updated doctor not found after save and detail fetch."); 

                _logger.LogInformation($"Doctor '{updatedDoctorWithDetails.FirstName} {updatedDoctorWithDetails.LastName}' (ID: {updatedDoctorWithDetails.Id}) updated successfully for Hospital {hospitalId}.");
                return _mapper.Map<DoctorProfileDto>(updatedDoctorWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating doctor with ID: {doctorId} for Hospital {hospitalId}.");
                throw; // Re-throw to controller.
            }
        }

        public async Task<DoctorProfileDto> DeleteDoctorAsync(string doctorId, string hospitalId)
        {
            try
            {
                // 1. Retrieve the doctor by ID with details.
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning($"Doctor with ID {doctorId} not found for deletion.");
                    throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");
                }

                // 2. Validate doctor's affiliation to the requesting hospital admin.
                // Ensures that the admin can only delete doctors belonging to their hospital.
                if (doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
                {
                    _logger.LogWarning($"Hospital Admin {hospitalId} attempted to delete doctor {doctorId} not affiliated with their hospital.");
                    throw new UnauthorizedAccessException($"Doctor with ID {doctorId} is not affiliated with your hospital.");
                }

                // 3. Soft delete: Set doctor's status to Deactivated.
                if (doctor.Status == Status.Deactivated)
                {
                    _logger.LogInformation($"Doctor {doctorId} is already deactivated. No action needed.");
                    return _mapper.Map<DoctorProfileDto>(doctor); // Return existing (deactivated) DTO.
                }

                doctor.Status = Status.Deactivated;
                // Update the Identity user's status and save changes via UserManager.
                var userUpdateResult = await _userManager.UpdateAsync(doctor);
                if (!userUpdateResult.Succeeded)
                {
                    _logger.LogError($"Failed to deactivate Identity user for doctor {doctorId}: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                    throw new InvalidOperationException($"Failed to deactivate doctor account: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                }

                // No separate _unitOfWork.Doctors.Update(doctor) is strictly needed here if UserManager.UpdateAsync
                // already tracks and saves the entity, but including it for clarity if other UoW changes are pending.
                await _unitOfWork.CompleteAsync(); // Commit all pending changes.

                _logger.LogInformation($"Doctor '{doctor.FirstName} {doctor.LastName}' (ID: {doctor.Id}) soft-deleted successfully for Hospital {hospitalId}.");
                return _mapper.Map<DoctorProfileDto>(doctor); // Return the updated (deactivated) doctor profile.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting doctor with ID: {doctorId} for Hospital {hospitalId}.");
                throw; // Re-throw to controller.
            }
        }



        public async Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsPerHospitalSpecialty(int hospitalSpecialtyId, PaginationParameters paginationParameters)
        {
            try
            {
                // 1. Get doctors for the specific HospitalSpecialty.

                var doctors = await _unitOfWork.Doctors.GetDoctorsByHospitalSpecialtyIdAsync(hospitalSpecialtyId);

                // 2. Apply search term filter in service (if needed for website search on displayed doctor fields)
                var query = doctors.AsQueryable();
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(term) ||
                        d.LastName.ToLower().Contains(term) 
                    );
                }

                // 3. Get total count AFTER applying filters
                var totalCount = query.Count();

                // 4. Apply pagination
                var pagedDoctors = query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                // 5. Map to DTOs
                var doctorDtos = _mapper.Map<IEnumerable<DoctorProfileDto>>(pagedDoctors);

                return new PagedResponseDto<DoctorProfileDto>(doctorDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctors for Hospital Specialty ID: {hospitalSpecialtyId}.");
                return new PagedResponseDto<DoctorProfileDto>(Enumerable.Empty<DoctorProfileDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }



        public async Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsPerHospitalSpecialty(string hospitalId, int specialtyId, PaginationParameters paginationParameters)
        {
            try
            {
                var hospitalSpecialty = await _unitOfWork.HospitalSpecialties.GetByHospitalAndSpecialtyIdAsync(hospitalId, specialtyId);
                if (hospitalSpecialty == null || !hospitalSpecialty.IsActive)
                {
                    _logger.LogWarning($"Doctor search failed: Hospital Specialty link for Hospital {hospitalId} and Specialty {specialtyId} not found or is inactive.");
                    return new PagedResponseDto<DoctorProfileDto>(Enumerable.Empty<DoctorProfileDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
                }

                var doctors = await _unitOfWork.Doctors.GetDoctorsByHospitalSpecialtyIdAsync(hospitalSpecialty.Id); // Only active doctors

                IQueryable<Doctor> query = doctors.AsQueryable();
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(d =>
                        d.FirstName.ToLower().Contains(term) ||
                        d.LastName.ToLower().Contains(term) ||
                        (d.Bio != null && d.Bio.ToLower().Contains(term)) ||
                        (d.Qualification != null && d.Qualification.ToLower().Contains(term))
                    );
                }

                var totalCount = query.Count();

                var pagedDoctors =  query
                    .OrderBy(d => d.LastName) // Default sorting
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                var doctorDtos = _mapper.Map<IEnumerable<DoctorProfileDto>>(pagedDoctors);
                return new PagedResponseDto<DoctorProfileDto>(doctorDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctors per Hospital {hospitalId} and Specialty {specialtyId}.");
                return new PagedResponseDto<DoctorProfileDto>(Enumerable.Empty<DoctorProfileDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }
    }
}