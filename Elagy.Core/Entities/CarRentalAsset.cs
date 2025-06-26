using Elagy.Core.Enums;
using Elagy.Core.Entities;


namespace Elagy.Core.Entities
{
    public class CarRentalAsset : Asset
    {
        // governates
        //public ICollection<Governorate> OperationalAreas { get; set; } // Geographic areas where car rental operates

        public FuelType[] FuelTypes { get; set; } 

        //Ai generated
        public string[] Models { get; set; }  // Specific models available for rent (e.g., Toyota Camry, Ford Explorer)

        public TransmissionType Transmission { get; set; }   // "Automatic", "Manual"
        public string[] RentalPolicies { get; set; } // (e.g., age restrictions, insurance requirements, mileage limits)

        
    }
}