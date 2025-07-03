using AutoMapper;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Elagy.BL.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecialtyService> _logger;

        public SpecialtyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SpecialtyService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        #region Supper Admin Dashboard CRUD

        public async Task<PagedResponseDto<SpecialtyResponseDto>> GetAllSpecialties(PaginationParameters paginationParameters) // MODIFIED: Accepts paginationParameters
        {
            try
            {

                IQueryable<Specialty> query = _unitOfWork.Specialties.AsQueryable();

                // 1. Apply SearchTerm filter (case-insensitive)
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string searchTermLower = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(s =>
                        s.Name.ToLower().Contains(searchTermLower) ||
                        (s.Description != null && s.Description.ToLower().Contains(searchTermLower))
                    );
                }

                // 2. Apply UserStatus filter (maps to Specialty.IsActive)
                if (paginationParameters.UserStatus.HasValue)
                {
                    bool targetIsActive = (paginationParameters.UserStatus.Value == Status.Active);
                    query = query.Where(s => s.IsActive == targetIsActive);
                }

                if (paginationParameters.SpecialtyId.HasValue)
                {
                   
                    query = query.Where(s => s.Id==paginationParameters.SpecialtyId.Value);
                }
                // If UserStatus is null, no IsActive filter is applied, returning both active and inactive specialties.

                // 3. Get total count AFTER applying filters but BEFORE pagination
                var totalCount = await query.CountAsync();

                // 4. Apply Pagination (Skip and Take)
                var specialties = await query
                    .OrderBy(s => s.Name) // Add a default sorting order for consistent pagination
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // 5. Map entities to DTOs
                var specialtyDtos = _mapper.Map<IEnumerable<SpecialtyResponseDto>>(specialties);

                // 6. Create and return PagedResponseDto
                var pagedResponse = new PagedResponseDto<SpecialtyResponseDto>(
                    specialtyDtos,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );

                _logger.LogInformation($"Successfully retrieved paginated specialties for Super Admin Dashboard. Page: {paginationParameters.PageNumber}, Size: {paginationParameters.PageSize}, Total: {totalCount}");
                return pagedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated specialties for Super Admin Dashboard.");
                // Return an empty paginated response on error
                return new PagedResponseDto<SpecialtyResponseDto>(Enumerable.Empty<SpecialtyResponseDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }

        public async Task<SpecialtyResponseDto> CreateSpecialty(SpecialtyCreateDto createDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Specialty creation failed: Name is required.");
                    throw new ArgumentException("Specialty name is required.");
                }

                var existingSpecialty = await _unitOfWork.Specialties.FindAsync(s => s.Name.ToLower() == createDto.Name.ToLower());
                if (existingSpecialty.Any())
                {
                    _logger.LogWarning($"Specialty creation failed: Specialty with name '{createDto.Name}' already exists.");
                    throw new InvalidOperationException($"Specialty with name '{createDto.Name}' already exists.");
                }

                var specialty = _mapper.Map<Specialty>(createDto);
                specialty.IsActive = true;

                await _unitOfWork.Specialties.AddAsync(specialty);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialty.Id}) created successfully.");
                return _mapper.Map<SpecialtyResponseDto>(specialty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating specialty.");
                throw;
            }
        }

        public async Task<SpecialtyResponseDto> DeleteSpecialtyAsync(int id)
        {
            try
            {
            
                var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
                if (specialty == null)
                {
                    _logger.LogWarning($"Specialty with ID {id} not found for deletion or is already inactive.");
                    return null;
                }

                if (!specialty.IsActive)
                {
                    _logger.LogInformation($"Specialty with ID {id} is already inactive. No action needed.");
                    return _mapper.Map<SpecialtyResponseDto>(specialty);
                }

                specialty.IsActive = false;
                _unitOfWork.Specialties.Update(specialty);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialty.Id}) soft-deleted successfully.");
                return _mapper.Map<SpecialtyResponseDto>(specialty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting specialty with ID {id}.");
                throw;
            }
        }

        public async Task<SpecialtyResponseDto> UpdateSpecialty(int specialtyId, SpecialtyUpdateDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Attempting to update Specialty with ID: {specialtyId}");

                // 1. Retrieve the existing entity using the 'id' parameter.
                var specialtyToUpdate = await _unitOfWork.Specialties.GetSpecialtyIdAsync(specialtyId);

                if (specialtyToUpdate == null)
                {
                    _logger.LogWarning($"Update failed: Specialty with ID {specialtyId} not found or is inactive.");
                    throw new KeyNotFoundException($"Specialty with ID {specialtyId} not found or is inactive.");
                }

                // 2. Implement Business Logic / Validation
 
                if (!string.Equals(specialtyToUpdate.Name, updateDto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Specialty name changed from '{specialtyToUpdate.Name}' to '{updateDto.Name}'. Checking for uniqueness.");
                    var existingSpecialtyWithSameName = await _unitOfWork.Specialties.FindAsync(s => s.Name.ToLower() == updateDto.Name.ToLower());

           
                    if (existingSpecialtyWithSameName.Any(s => s.Id != specialtyId))
                    {
                        _logger.LogWarning($"Update failed: Specialty with name '{updateDto.Name}' already exists for another ID.");
                        throw new InvalidOperationException($"Specialty with name '{updateDto.Name}' already exists.");
                    }
                }

                // 3. Map DTO properties to the existing entity.
                _mapper.Map(updateDto, specialtyToUpdate);


                // 4. Update the entity in the repository (marks it for update in EF Core context).
                _unitOfWork.Specialties.Update(specialtyToUpdate);

                // 5. Commit the changes to the database.
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Specialty '{specialtyToUpdate.Name}' (ID: {specialtyToUpdate.Id}) updated successfully.");

                // 6. Map the updated entity back to a response DTO and return.
                return _mapper.Map<SpecialtyResponseDto>(specialtyToUpdate);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Specialty update failed: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Business rule violation during specialty update for ID {specialtyId}.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while updating specialty with ID {specialtyId}.");
                throw;
            }
        }



        #endregion

        
        public async Task<SpecialtyLinkToHospitalDto?> LinkSpecialtyToHospital(int specialtyId, string hospitalId)
        {
            try
            {
                _logger.LogInformation($"Attempting to link Specialty ID {specialtyId} to Hospital ID {hospitalId}.");

                // 1. Retrieve Specialty (must be active as per GetByIdAsync's implicit filter)
                var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(specialtyId);
                if (specialty == null)
                {
                    _logger.LogWarning($"Link failed: Specialty with ID {specialtyId} not found or is inactive.");
                    return null; // Or throw KeyNotFoundException
                }

                // 2. Retrieve HospitalAsset
                var hospitalAsset = await _unitOfWork.HospitalAssets.GetByIdAsync(hospitalId);
                if (hospitalAsset == null)
                {
                    _logger.LogWarning($"Link failed: Hospital Asset with ID {hospitalId} not found.");
                    return null; // Or throw KeyNotFoundException
                }

                // 3. Check if the link already exists
                var existingLink = await _unitOfWork.HospitalSpecialties.GetByHospitalAndSpecialtyIdAsync(hospitalId, specialtyId);
                if (existingLink != null)
                {
                    _logger.LogInformation($"Link already exists between Specialty '{specialty.Name}' (ID: {specialtyId}) and Hospital '{hospitalAsset.Name}' (ID: {hospitalId}).");
                    // Populate navigation properties for mapping
                    existingLink.Specialty = specialty;
                    existingLink.HospitalAsset = hospitalAsset;
                    return _mapper.Map<SpecialtyLinkToHospitalDto>(existingLink);
                }

                // 4. Create a new HospitalSpecialty link
                var hospitalSpecialty = new HospitalSpecialty
                {
                    SpecialtyId = specialtyId,
                    HospitalAssetId = hospitalId,
                    // If HospitalSpecialty entity should have its own IsActive status for the link:
                    // IsActive = true
                };

                await _unitOfWork.HospitalSpecialties.AddAsync(hospitalSpecialty);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialtyId}) linked to Hospital '{hospitalAsset.Name}' (ID: {hospitalId}) successfully.");

                // 5. Populate navigation properties for mapping the *newly created* link DTO
                hospitalSpecialty.Specialty = specialty;
                hospitalSpecialty.HospitalAsset = hospitalAsset;

                // 6. Map to DTO and explicitly set Status from the global Specialty's IsActive
                var resultDto = _mapper.Map<SpecialtyLinkToHospitalDto>(hospitalSpecialty);
                resultDto.Status = specialty.IsActive ? Status.Active : Status.Deactivated;
                return resultDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error linking Specialty ID {specialtyId} to Hospital ID {hospitalId}.");
                throw;
            }
        }

     
        /// Retrieves all active specialties linked to a specific hospital.
       
        public async Task<PagedResponseDto<SpecialtyResponseDto>> GetAllSpecialtiesForHospital(string hospitalId)
        {
            try
            {
                _logger.LogInformation($"Retrieving specialties for Hospital ID: {hospitalId}.");

                // Calls repository method GetSpecialtiesByHospitalIdAsync(isActive: true),
                // which means it returns only active specialties linked to the hospital.
                var specialties = await _unitOfWork.Specialties.GetSpecialtiesByHospitalIdAsync(hospitalId, isActive: true);

                var totalCount = specialties.Count();
                var pagedResponse = new PagedResponseDto<SpecialtyResponseDto>(
                    _mapper.Map<IEnumerable<SpecialtyResponseDto>>(specialties),
                    totalCount,
                    1,
                    totalCount > 0 ? totalCount : 1
                );

                _logger.LogInformation($"Successfully retrieved {totalCount} active specialties for Hospital ID: {hospitalId}.");
                return pagedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving specialties for Hospital ID: {hospitalId}.");
                return new PagedResponseDto<SpecialtyResponseDto>(Enumerable.Empty<SpecialtyResponseDto>(), 0, 1, 10);
            }
        }


       

        /// Retrieves active global specialties that are not yet linked to a specific hospital.
        /// The description field is explicitly omitted in the response DTO.
        public async Task<IEnumerable<SpecialtyResponseDto>> GetAvailableGlobalSpecialtiesToLinkAsync(string hospitalId)
        {
            try
            {
                _logger.LogInformation($"Retrieving available global specialties to link for Hospital ID: {hospitalId}.");

                // Calls repository method GetUnlinkedSpecialtiesForHospitalAsync,
                // which implicitly filters for IsActive = true.
                var specialties = await _unitOfWork.Specialties.GetUnlinkedSpecialtiesForHospitalAsync(hospitalId);

                // Map to DTO, explicitly omitting description as requested.
                var specialtyDtos = specialties.Select(s => new SpecialtyResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = null // Explicitly set to null as per the service method's comment
                }).ToList();

                _logger.LogInformation($"Successfully retrieved {specialtyDtos.Count} available active global specialties to link for Hospital ID: {hospitalId}.");
                return specialtyDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving available global specialties to link for Hospital ID: {hospitalId}.");
                return Enumerable.Empty<SpecialtyResponseDto>();
            }
        }

        public async Task<SpecialtyResponseDto?> GetSpecialtyIdAsync(int id)
        {
            try
            {
                // This calls the repository method that filters by IsActive=true
                var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
                if (specialty == null)
                {
                    _logger.LogInformation($"Specialty with ID {id} not found or is inactive in repository.");
                    return null;
                }
                return _mapper.Map<SpecialtyResponseDto>(specialty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving specialty by ID {id}.");
                throw;
            }
        }

        public async Task<SpecialtyResponseDto?> ChangeSpecialtyStatusAsync(int id, bool newIsActiveStatus)
        {
            try
            {
                var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);

                if (specialty == null)
                {
                    _logger.LogWarning($"Specialty with ID {id} not found for status change.");
                    return null; // Specialty genuinely not found
                }

                // If the status is already the target status, no action needed.
                if (specialty.IsActive == newIsActiveStatus)
                {
                    _logger.LogInformation($"Specialty with ID {id} is already in the target status ({newIsActiveStatus}). No action needed.");
                    return _mapper.Map<SpecialtyResponseDto>(specialty);
                }

                specialty.IsActive = newIsActiveStatus; // Set the new status
                _unitOfWork.Specialties.Update(specialty); // Mark for update
                await _unitOfWork.CompleteAsync(); // Save changes

                _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialty.Id}) status changed to {newIsActiveStatus} successfully.");
                return _mapper.Map<SpecialtyResponseDto>(specialty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing status for specialty with ID {id} to {newIsActiveStatus}.");
                throw;
            }
        }

        public async Task<SpecialtyLinkToHospitalDto?> ChangeSpecificHospitalSpecialtyStatusAsync(
            string hospitalId,
            int specialtyId,
            Status newStatus)
        {
            try
            {
                _logger.LogInformation($"Changing status of specialty link {specialtyId} for Hospital {hospitalId} to {newStatus}");

                bool newIsActiveStatus = newStatus == Status.Active;

                // Update status
                bool updated = await _unitOfWork.HospitalSpecialties
                    .UpdateHospitalSpecialtyLinkStatusAsync(hospitalId, specialtyId, newIsActiveStatus);

                if (!updated)
                {
                    _logger.LogWarning($"Specialty link {specialtyId} not found or not affiliated with Hospital {hospitalId}");
                    return null;
                }

                await _unitOfWork.CompleteAsync();

                // Fetch updated entity with includes
                var updatedLink = await _unitOfWork.HospitalSpecialties
                    .GetByHospitalAndSpecialtyIdAsync(
                        hospitalId,
                        specialtyId,
                        includes: q => q
                            .Include(hs => hs.Specialty)
                            .Include(hs => hs.HospitalAsset)
                    );

                if (updatedLink == null)
                {
                    _logger.LogError($"Updated specialty link {specialtyId} not found after successful update");
                    return null;
                }

                return _mapper.Map<SpecialtyLinkToHospitalDto>(updatedLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing status of specialty link {specialtyId}");
                throw;
            }
        }
    }
 }

