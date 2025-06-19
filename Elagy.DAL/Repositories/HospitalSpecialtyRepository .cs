using Elagy.Core.Entities;
using Elagy.Core.IRepositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class HospitalSpecialtyRepository : GenericRepository<HospitalSpecialty>, IHospitalSpecialtyRepository
    {
        public HospitalSpecialtyRepository(ApplicationDbContext context) : base(context) { }
    }
}
