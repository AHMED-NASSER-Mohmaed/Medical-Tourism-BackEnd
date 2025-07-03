using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices.ISpecialtyService
{
    public interface ISpecialtyService
    {


        #region Supper Admin Dashboard CRUD
        Task<PagedResponseDto<SpecialtyResponseDto>> GetAllSpecialties(PaginationParameters paginationParameters);
        Task<SpecialtyResponseDto> CreateSpecialty(SpecialtyCreateDto createDto);
        Task<SpecialtyResponseDto> DeleteSpecialtyAsync(int id);
        Task<SpecialtyResponseDto> UpdateSpecialty(int specialtyId,SpecialtyUpdateDto updateDto);
        #endregion
        Task<SpecialtyResponseDto?> ChangeSpecialtyStatusAsync(int id, bool newIsActiveStatus);
        Task<SpecialtyResponseDto?> GetSpecialtyIdAsync(int id);
        Task<SpecialtyLinkToHospitalDto?> LinkSpecialtyToHospital(int specialtyId, string hospitalId);

        //<summary>
        //for supper admin dashboard and external site 
        //<summary>
        Task<PagedResponseDto<SpecialtyResponseDto>> GetAllSpecialtiesForHospital(string hospitalId, PaginationParameters paginationParameters);
        Task<SpecialtyLinkToHospitalDto?> ChangeSpecificHospitalSpecialtyStatusAsync(string hospitalId, int specialtyId, Status newStatus);
        // do not return description
        Task<IEnumerable<SpecialtyResponseDto>> GetAvailableGlobalSpecialtiesToLinkAsync(string hospitalId);


        Task<PagedResponseDto<SpecialtyLinkToHospitalDto>> GetHospitalSpecialtiesWithLinks(
    string hospitalId,
    PaginationParameters paginationParameters);

    }
}
