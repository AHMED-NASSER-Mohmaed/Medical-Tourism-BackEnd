using Elagy.Core.DTOs.SpecialtyDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices.ISpecialtyService
{
    public interface ISpecialtyService
    {
        //Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync(); // Get ALL global specialties
        //Task<SpecialtyDto> GetSpecialtyByIdAsync(int id); // Get a global specialty by ID
        //Task<SpecialtyDto> CreateSpecialtyAsync(SpecialtyCreateDto createDto); // Create a global specialty
        //Task<bool> UpdateSpecialtyAsync(SpecialtyUpdateDto updateDto); // Update a global specialty
        //Task<bool> DeleteSpecialtyAsync(int id); // Delete a global specialty
        // Admin Dashboard CRUD
        Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync();
        Task<SpecialtyDto> GetSpecialtyByIdAsync(int id);
        Task<SpecialtyDto> CreateSpecialtyAsync(SpecialtyCreateDto createDto);
        Task<bool> UpdateSpecialtyAsync(SpecialtyUpdateDto updateDto);
        Task<bool> DeleteSpecialtyAsync(int id);

        Task<IEnumerable<SpecialtyDto>> GetSpecialtiesForHospitalAdminDashboardAsync(string hospitalId);
        Task<IEnumerable<SpecialtyDto>> GetAvailableGlobalSpecialtiesToLinkAsync(string hospitalId);
        Task<bool> LinkSpecialtyToHospitalAsync(SpecialtyLinkToHospitalDto linkDto, string hospitalId); // Hospital Admin links an existing specialty
        Task<bool> RemoveSpecialtyFromHospitalAsync(int specialtyId, string hospitalId); // Hospital Admin removes specialty from hospital's offerings
        Task<bool> IsSpecialtyAssociatedWithHospitalAsync(int specialtyId, string hospitalId); // Helper to check association
        

        // Website View (e.g., patient viewing all specialties on the website)
        //Task<IEnumerable<SpecialtyWebsiteDto>> GetAllWebsiteSpecialtiesAsync();
    }
}
