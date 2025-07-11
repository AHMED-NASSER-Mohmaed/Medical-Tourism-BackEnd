using Elagy.Core.DTOs.Package;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IPackgeService
    {
        Task<Package> CreatePackage(string _patientId);
        Task<PagedResponseDto<PackageResponseDTO>> getPackages(string _patientId, PaginationParameters pp);

        Task<PackageWithDetailDTO> GetPackageDetails(Guid packageId);

    }

}
