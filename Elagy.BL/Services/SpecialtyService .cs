using AutoMapper;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices.ISpecialtyService;
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
                // Start with an IQueryable to apply filters and pagination efficiently at the database level.
                // Assuming _unitOfWork.Specialties.AsQueryable() provides an IQueryable<Specialty>.
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

        /// <summary>
        /// Links an active specialty to a hospital.
        /// </summary>
        /// <param name="specialtyId">The ID of the specialty to link.</param>
        /// <param name="hospitalId">The ID of the hospital to link to.</param>
        /// <returns>A SpecialtyLinkToHospitalDto if successful, or null if specialty/hospital not found or link already exists.</returns>
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


        public async Task<SpecialtyLinkToHospitalDto> CahngeSTSpecialtiesForHospitalAdminDashboard(string hospitalId, Status St)
        {
            try
            {
                _logger.LogInformation($"Attempting to change status of specialties for Hospital ID {hospitalId} to {St}.");

                // 1. Get all specialties linked to this hospital (regardless of their current active status).
                var linkedSpecialties = await _unitOfWork.Specialties.GetSpecialtiesByHospitalIdAsync(hospitalId, isActive: false);

                bool targetIsActive = (St == Status.Active);

                // 2. Iterate and update each linked specialty's global IsActive status.
                foreach (var specialty in linkedSpecialties)
                {
                    if (specialty.IsActive != targetIsActive) // Only update if a change is needed
                    {
                        specialty.IsActive = targetIsActive;
                        _unitOfWork.Specialties.Update(specialty);
                    }
                }

                // 3. Commit all changes to the database.
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Changed global active status of {linkedSpecialties.Count()} specialties linked to Hospital ID {hospitalId} to {St}.");

                // Returning a single DTO for a batch operation is not ideal.
                // Returning a placeholder to match the current service signature.
                return new SpecialtyLinkToHospitalDto
                {
                    HospitalId = hospitalId,
                    SpecialtyId = 0, // Placeholder as no single specialty is represented
                    Status = St,
                    SpecialtyName = "Batch operation completed", // Informative message
                    SpecialtyDesc = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing specialties status for Hospital ID {hospitalId} to {St}.");
                throw;
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
    }
}


#region old implemetatipn

//public async Task<SpecialtyResponseDto> CreateSpecialtyAsync(SpecialtyCreateDto createDto)
//{
//    if (await _unitOfWork.Specialties.ExistsByNameAsync(createDto.Name))
//    {
//        throw new ArgumentException($"Specialty with name '{createDto.Name}' already exists globally.");
//    }
//    Specialty sp = new Specialty()
//    {
//        Name = createDto.Name,
//        Description = createDto.Description,
//    };

//    await _unitOfWork.Specialties.AddAsync(sp);
//    await _unitOfWork.CompleteAsync();
//    _logger.LogInformation($"Specialty '{sp.Name}' created with ID: {sp.Id}");
//    Specialty spdto = new Specialty()
//    {
//        Name = sp.Name,
//        Description = sp.Description,
//    };
//    return _mapper.Map<SpecialtyResponseDto>(spdto);
//}

//public async Task<bool> DeleteSpecialtyAsync(int id)
//{
//    var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
//    if (specialty == null)
//    {
//        _logger.LogWarning($"Specialty with ID {id} not found for global deletion.");
//        return false;
//    }

//    var isAssociated = await (await _unitOfWork.Hospitals.GetHospitalsBySpecialtyQueryAsync(id)).AnyAsync();
//    if (isAssociated)
//    {
//        _logger.LogWarning($"Attempted global delete of Specialty ID {id} with existing HospitalSpecialty links.");
//        throw new InvalidOperationException("Cannot delete specialty globally as it is currently associated with one or more hospitals. Remove all hospital associations first.");
//    }

//    _unitOfWork.Specialties.AddAsync(specialty);
//    await _unitOfWork.CompleteAsync();
//    _logger.LogInformation($"Global Specialty ID {id} deleted successfully by SuperAdmin.");
//    return true;
//}


//public async Task<IEnumerable<SpecialtyResponseDto>> GetAllSpecialtiesAsync()
//{
//    var specialties = await _unitOfWork.Specialties.GetAllAsync();
//    List < SpecialtyResponseDto> spdTOS = new List<SpecialtyResponseDto>();
//    foreach (var specialty in specialties)
//    {
//     var stdo= _mapper.Map<SpecialtyResponseDto>(specialty);
//        spdTOS.Add(stdo);

//    }
//    return spdTOS;
//}

//public async Task<IEnumerable<SpecialtyResponseDto>> GetAvailableGlobalSpecialtiesToLinkAsync(string hospitalId)
//{
//    var allGlobalSpecialties = await _unitOfWork.Specialties.GetAllAsync();

//    // 2. Get specialties already linked to this hospital
//    var linkedSpecialtyIds = await _unitOfWork.Hospitals.AsQueryable()
//                                                         .SelectMany(h => h.HospitalSpecialties)
//                                                         .Where(hs => hs.HospitalAssetId == hospitalId)
//                                                         .Select(hs => hs.SpecialtyId)
//                                                         .ToListAsync();

//    // 3. Filter out already linked specialties
//    var availableSpecialties = allGlobalSpecialties
//                                .Where(s => !linkedSpecialtyIds.Contains(s.Id));

//    List<SpecialtyResponseDto> spdTOS = new List<SpecialtyResponseDto>();
//    foreach (var specialty in availableSpecialties)
//    {
//        SpecialtyResponseDto specialtyDto = new SpecialtyResponseDto()
//        {
//            Description = specialty.Description,
//            Name = specialty.Name,
//        };
//        spdTOS.Add(specialtyDto);

//    }
//    return spdTOS;
//}

//public async Task<IEnumerable<SpecialtyResponseDto>> GetSpecialtiesForHospitalAdminDashboardAsync(string hospitalId)
//{
//    var hospitalSpecialties = await _unitOfWork.Hospitals.GetHospitalSpecialtiesInHospitalAsync(hospitalId);
//    if (hospitalSpecialties == null || !hospitalSpecialties.Any())
//    {
//        _logger.LogInformation($"No specialties found for hospital admin dashboard with hospital ID: {hospitalId}");
//        return new List<SpecialtyResponseDto>();
//    }
//    var specialtyDtos = hospitalSpecialties.Select(hs => new SpecialtyResponseDto
//    {
//        Name = hs.Specialty.Name,
//        Description = hs.Specialty.Description
//    }).ToList();

//    return specialtyDtos;
//}

//public async Task<SpecialtyResponseDto> GetSpecialtyByIdAsync(int id)
//{
//    var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
//    return _mapper.Map<SpecialtyResponseDto>(specialty);
//}

//public async Task<bool> IsSpecialtyAssociatedWithHospitalAsync(int specialtyId, string hospitalId)
//{
//    return await _unitOfWork.HospitalSpecialties.AsQueryable() 
//                     .AnyAsync(hs => hs.SpecialtyId == specialtyId && hs.HospitalAssetId == hospitalId);
//}

//public async Task<bool> LinkSpecialtyToHospitalAsync(SpecialtyLinkToHospitalDto linkDto, string hospitalId)
//{
//    var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(linkDto.SpecialtyId); // Corrected typo here from GetSpecialtyIdAsync
//    if (specialty == null)
//    {
//        _logger.LogWarning($"Hospital Admin {hospitalId} attempted to link non-existent Specialty ID: {linkDto.SpecialtyId}.");
//        throw new ArgumentException($"Specialty with ID {linkDto.SpecialtyId} not found.");
//    }

//    var hospitalAsset = await _unitOfWork.Hospitals.GetByIdAsync(hospitalId);
//    if (hospitalAsset == null)
//    {
//        _logger.LogError($"Authenticated Hospital Admin's hospital ID {hospitalId} does not map to a valid HospitalAsset.");
//        throw new ArgumentException($"Invalid hospital ID {hospitalId} for current admin.");
//    }

//    var isAlreadyLinkedToHospital = await IsSpecialtyAssociatedWithHospitalAsync(linkDto.SpecialtyId, hospitalId);

//    if (isAlreadyLinkedToHospital)
//    {
//        _logger.LogWarning($"Specialty ID {linkDto.SpecialtyId} is already linked to Hospital ID {hospitalId}.");
//        throw new InvalidOperationException($"Specialty '{specialty.Name}' is already listed for your hospital.");
//    }

//    var hospitalSpecialty = new HospitalSpecialty
//    {
//        HospitalAssetId = hospitalId,
//        SpecialtyId = linkDto.SpecialtyId
//    };
//    // THIS IS THE CORRECTED LINE:
//    await _unitOfWork.HospitalSpecialties.AddAsync(hospitalSpecialty); // <--- Use the new HospitalSpecialties repo
//    await _unitOfWork.CompleteAsync();

//    _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialty.Id}) successfully linked to Hospital ID: {hospitalId} by Hospital Admin.");
//    return true;
//}

//public async Task<bool> RemoveSpecialtyFromHospitalAsync(int specialtyId, string hospitalId)
//{

//    var hospitalSpecialtyLink = await _unitOfWork.HospitalSpecialties.AsQueryable() // <--- Use the HospitalSpecialties repo
//                                                   .Where(hs => hs.SpecialtyId == specialtyId && hs.HospitalAssetId == hospitalId)
//                                                   .FirstOrDefaultAsync();

//    if (hospitalSpecialtyLink == null)
//    {
//        _logger.LogWarning($"Hospital Admin {hospitalId} attempted to remove Specialty ID {specialtyId}, but no such link exists for their hospital.");
//        return false;
//    }

//    var doctorsLinkedToThisSpecificHospitalSpecialty = await _unitOfWork.Doctors.AsQueryable()
//                                                                        .AnyAsync(d => d.HospitalSpecialtyId == hospitalSpecialtyLink.Id);

//    if (doctorsLinkedToThisSpecificHospitalSpecialty)
//    {
//        _logger.LogWarning($"Attempted to remove Specialty ID {specialtyId} from Hospital ID {hospitalId} while doctors are still linked to this specific hospital-specialty context.");
//        throw new InvalidOperationException("Cannot remove specialty from your hospital as doctors are still assigned to this specialty within your hospital. Please reassign or remove doctors first.");
//    }

//    _unitOfWork.HospitalSpecialties.Remove(hospitalSpecialtyLink); 
//    await _unitOfWork.CompleteAsync();
//    _logger.LogInformation($"Specialty ID {specialtyId} successfully removed from Hospital ID: {hospitalId} by Hospital Admin.");
//    return true;
//}

//public async Task<bool> UpdateSpecialtyAsync(SpecialtyUpdateDto updateDto)
//{
//    var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(updateDto.Id);
//    if (specialty == null)
//    {
//        _logger.LogWarning($"Specialty with ID {updateDto.Id} not found for update.");
//        return false;
//    }
//    if (specialty.Name != updateDto.Name && await _unitOfWork.Specialties.ExistsByNameAsync(updateDto.Name))
//    {
//        throw new ArgumentException($"Specialty with name '{updateDto.Name}' already exists globally.");
//    }

//    _mapper.Map(updateDto, specialty);
//    _unitOfWork.Specialties.Update(specialty);
//    await _unitOfWork.CompleteAsync();
//    _logger.LogInformation($"Specialty ID {specialty.Id} updated.");
//    return true;
#endregion
