using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class CarRentalProviderRegistrationRequestDto : BaseServiceProviderRegistrationRequestDto
    {
        public string OperationalAreas { get; set; }
        public string[] VehicleType { get; set; }
        public string[] Transmission { get; set; }
        public string[] FuelType { get; set; }
        public string[] RentalPolicies { get; set; }
        public string[] AdditionalServices { get; set; }
        public string[] CarFeatures { get; set; }

        public CarRentalProviderRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.CarRental; // Default for this DTO
        }
    }
}