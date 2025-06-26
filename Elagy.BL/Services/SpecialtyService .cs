using AutoMapper;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices.ISpecialtyService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<SpecialtyDto> CreateSpecialtyAsync(SpecialtyCreateDto createDto)
        {
            if (await _unitOfWork.Specialties.ExistsByNameAsync(createDto.Name))
            {
                throw new ArgumentException($"Specialty with name '{createDto.Name}' already exists globally.");
            }
            Specialty sp = new Specialty()
            {
                Name = createDto.Name,
                Description = createDto.Description,
            };
         
            await _unitOfWork.Specialties.AddAsync(sp);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Specialty '{sp.Name}' created with ID: {sp.Id}");
            Specialty spdto = new Specialty()
            {
                Name = sp.Name,
                Description = sp.Description,
            };
            return _mapper.Map<SpecialtyDto>(spdto);
        }

        public async Task<bool> DeleteSpecialtyAsync(int id)
        {
            var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
            if (specialty == null)
            {
                _logger.LogWarning($"Specialty with ID {id} not found for global deletion.");
                return false;
            }

            var isAssociated = await (await _unitOfWork.Hospitals.GetHospitalsBySpecialtyQueryAsync(id)).AnyAsync();
            if (isAssociated)
            {
                _logger.LogWarning($"Attempted global delete of Specialty ID {id} with existing HospitalSpecialty links.");
                throw new InvalidOperationException("Cannot delete specialty globally as it is currently associated with one or more hospitals. Remove all hospital associations first.");
            }

            _unitOfWork.Specialties.AddAsync(specialty);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Global Specialty ID {id} deleted successfully by SuperAdmin.");
            return true;
        }


        public async Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync()
        {
            var specialties = await _unitOfWork.Specialties.GetAllAsync();
            List < SpecialtyDto> spdTOS = new List<SpecialtyDto>();
            foreach (var specialty in specialties)
            {
             var stdo= _mapper.Map<SpecialtyDto>(specialty);
                spdTOS.Add(stdo);
              
            }
            return spdTOS;
        }

        public async Task<IEnumerable<SpecialtyDto>> GetAvailableGlobalSpecialtiesToLinkAsync(string hospitalId)
        {
            var allGlobalSpecialties = await _unitOfWork.Specialties.GetAllAsync();

            // 2. Get specialties already linked to this hospital
            var linkedSpecialtyIds = await _unitOfWork.Hospitals.AsQueryable()
                                                                 .SelectMany(h => h.HospitalSpecialties)
                                                                 .Where(hs => hs.HospitalAssetId == hospitalId)
                                                                 .Select(hs => hs.SpecialtyId)
                                                                 .ToListAsync();

            // 3. Filter out already linked specialties
            var availableSpecialties = allGlobalSpecialties
                                        .Where(s => !linkedSpecialtyIds.Contains(s.Id));

            List<SpecialtyDto> spdTOS = new List<SpecialtyDto>();
            foreach (var specialty in availableSpecialties)
            {
                SpecialtyDto specialtyDto = new SpecialtyDto()
                {
                    Description = specialty.Description,
                    Name = specialty.Name,
                };
                spdTOS.Add(specialtyDto);

            }
            return spdTOS;
        }

        public async Task<IEnumerable<SpecialtyDto>> GetSpecialtiesForHospitalAdminDashboardAsync(string hospitalId)
        {
            var hospitalSpecialties = await _unitOfWork.Hospitals.GetHospitalSpecialtiesInHospitalAsync(hospitalId);
            if (hospitalSpecialties == null || !hospitalSpecialties.Any())
            {
                _logger.LogInformation($"No specialties found for hospital admin dashboard with hospital ID: {hospitalId}");
                return new List<SpecialtyDto>();
            }
            var specialtyDtos = hospitalSpecialties.Select(hs => new SpecialtyDto
            {
                Name = hs.Specialty.Name,
                Description = hs.Specialty.Description
            }).ToList();

            return specialtyDtos;
        }

        public async Task<SpecialtyDto> GetSpecialtyByIdAsync(int id)
        {
            var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(id);
            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task<bool> IsSpecialtyAssociatedWithHospitalAsync(int specialtyId, string hospitalId)
        {
            return await _unitOfWork.HospitalSpecialties.AsQueryable() 
                             .AnyAsync(hs => hs.SpecialtyId == specialtyId && hs.HospitalAssetId == hospitalId);
        }

        public async Task<bool> LinkSpecialtyToHospitalAsync(SpecialtyLinkToHospitalDto linkDto, string hospitalId)
        {
            var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(linkDto.SpecialtyId); // Corrected typo here from GetSpecialtyIdAsync
            if (specialty == null)
            {
                _logger.LogWarning($"Hospital Admin {hospitalId} attempted to link non-existent Specialty ID: {linkDto.SpecialtyId}.");
                throw new ArgumentException($"Specialty with ID {linkDto.SpecialtyId} not found.");
            }

            var hospitalAsset = await _unitOfWork.Hospitals.GetByIdAsync(hospitalId);
            if (hospitalAsset == null)
            {
                _logger.LogError($"Authenticated Hospital Admin's hospital ID {hospitalId} does not map to a valid HospitalAsset.");
                throw new ArgumentException($"Invalid hospital ID {hospitalId} for current admin.");
            }

            var isAlreadyLinkedToHospital = await IsSpecialtyAssociatedWithHospitalAsync(linkDto.SpecialtyId, hospitalId);

            if (isAlreadyLinkedToHospital)
            {
                _logger.LogWarning($"Specialty ID {linkDto.SpecialtyId} is already linked to Hospital ID {hospitalId}.");
                throw new InvalidOperationException($"Specialty '{specialty.Name}' is already listed for your hospital.");
            }

            var hospitalSpecialty = new HospitalSpecialty
            {
                HospitalAssetId = hospitalId,
                SpecialtyId = linkDto.SpecialtyId
            };
            // THIS IS THE CORRECTED LINE:
            await _unitOfWork.HospitalSpecialties.AddAsync(hospitalSpecialty); // <--- Use the new HospitalSpecialties repo
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Specialty '{specialty.Name}' (ID: {specialty.Id}) successfully linked to Hospital ID: {hospitalId} by Hospital Admin.");
            return true;
        }

        public async Task<bool> RemoveSpecialtyFromHospitalAsync(int specialtyId, string hospitalId)
        {

            var hospitalSpecialtyLink = await _unitOfWork.HospitalSpecialties.AsQueryable() // <--- Use the HospitalSpecialties repo
                                                           .Where(hs => hs.SpecialtyId == specialtyId && hs.HospitalAssetId == hospitalId)
                                                           .FirstOrDefaultAsync();

            if (hospitalSpecialtyLink == null)
            {
                _logger.LogWarning($"Hospital Admin {hospitalId} attempted to remove Specialty ID {specialtyId}, but no such link exists for their hospital.");
                return false;
            }

            var doctorsLinkedToThisSpecificHospitalSpecialty = await _unitOfWork.Doctors.AsQueryable()
                                                                                .AnyAsync(d => d.HospitalSpecialtyId == hospitalSpecialtyLink.Id);

            if (doctorsLinkedToThisSpecificHospitalSpecialty)
            {
                _logger.LogWarning($"Attempted to remove Specialty ID {specialtyId} from Hospital ID {hospitalId} while doctors are still linked to this specific hospital-specialty context.");
                throw new InvalidOperationException("Cannot remove specialty from your hospital as doctors are still assigned to this specialty within your hospital. Please reassign or remove doctors first.");
            }

            _unitOfWork.HospitalSpecialties.Remove(hospitalSpecialtyLink); 
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Specialty ID {specialtyId} successfully removed from Hospital ID: {hospitalId} by Hospital Admin.");
            return true;
        }

        public async Task<bool> UpdateSpecialtyAsync(SpecialtyUpdateDto updateDto)
        {
            var specialty = await _unitOfWork.Specialties.GetSpecialtyIdAsync(updateDto.Id);
            if (specialty == null)
            {
                _logger.LogWarning($"Specialty with ID {updateDto.Id} not found for update.");
                return false;
            }
            if (specialty.Name != updateDto.Name && await _unitOfWork.Specialties.ExistsByNameAsync(updateDto.Name))
            {
                throw new ArgumentException($"Specialty with name '{updateDto.Name}' already exists globally.");
            }

            _mapper.Map(updateDto, specialty);
            _unitOfWork.Specialties.Update(specialty);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Specialty ID {specialty.Id} updated.");
            return true;
        }
    }
}
