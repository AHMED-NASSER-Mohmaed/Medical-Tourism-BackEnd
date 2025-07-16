using Elagy.Core.DTOs.MlPrediction;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IMLRepository 
    {
     Task<List<HospitalRatingDto>> GetAlBookingPatient();
    }
}
