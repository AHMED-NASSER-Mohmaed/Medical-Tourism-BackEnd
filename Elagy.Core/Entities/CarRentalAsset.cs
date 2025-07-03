using Elagy.Core.Enums;
using Elagy.Core.Entities;


namespace Elagy.Core.Entities
{
    public class CarRentalAsset : Asset
    {

        public FuelType[] FuelTypes { get; set; } 
        public string[] Models { get; set; } 

        public TransmissionType Transmission { get; set; }   // "Automatic", "Manual"
        public string[] RentalPolicies { get; set; } // (e.g., age restrictions, insurance requirements, mileage limits)
        public ICollection<CarRentalAssetImage>? CarRentalAssetImages { get; set; }
        public ICollection<Car>? Cars { get; set; } 
        public ICollection<Driver>? Drivers { get; set; } 


    }
}