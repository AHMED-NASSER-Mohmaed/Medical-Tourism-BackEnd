using Elagy.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Elagy.Core.DTOs.Auth
{
    public class CarRentalAssetRegistrationRequestDto : BaseAssetRegistrationRequestDto
    {

        //[Required(ErrorMessage = "Please provide the operational areas for the car rental service.")]
        //public Governorate[] OperationalAreas { get; set; } // Geographic areas where car rental operates


        public FuelType[] FuelTypes { get; set; }

        //Ai generated
        public string[] Models { get; set; }  // Specific models available for rent (e.g., Toyota Camry, Ford Explorer)

        public TransmissionType Transmission { get; set; }   // "Automatic", "Manual"
        public string[] RentalPolicies { get; set; } // (e.g., age restrictions, insurance requirements, mileage limits)


        public CarRentalAssetRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.CarRental; // Default for this DTO
        }
    }
}