using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Specialty
{
    public class HospitalSpecialtyDto
    {
        public int Id { get; set; } // The artificial PK of the join table entry

        public string HospitalAssetId { get; set; } // The ID of the hospital
        public HospitalMinDto HospitalAsset { get; set; } // Nested DTO for hospital info

        public int SpecialtyId { get; set; } // The ID of the specialty
        public SpecialtyResponseDto Specialty { get; set; } // Nested DTO for specialty info
    }
}
