using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.MlPrediction
{
    public class HospitalRatingDto
    {

        //User Info
        public string UserId { get; set; }

        public string? Address { get; set; } // Full address as a single string
        public string? City { get; set; } // City name, not a complex object
        public int GovernorateId { get; set; } //governate forgin key

        public float Age { get; set; }
        public string BloodGroup { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }

        // Appointment Info
        public float Appointementprice { get; set; }
        // from SpecialtyAppointments
        public float SpecialtyScheduleId { get; set; }
        //  from HospitalSpecialty
       

        public string CarRentalAssetId { get; set; }
        public int HospitalSpecialtyId { get; set; }
        public string HospitalAssetId { get; set; } // Foreign Key to HospitalAsset

        public string HotelAssetId { get; set; }
        public float  Label {  get; set; }

    }
}
