namespace Elagy.Core.Entities
{
    public class CarRentalAsset : ServiceAsset
    {
        public string OperationalAreas { get; set; } // Geographic areas where car rental operates
        public string[] VehicleType { get; set; }    // Types of vehicles available (e.g., sedan, SUV, truck, luxury)
        public string[] Transmission { get; set; }   // "Automatic", "Manual"
        public string[] FuelType { get; set; }       // (e.g., petrol, diesel, electric)
        public string[] RentalPolicies { get; set; } // (e.g., age restrictions, insurance requirements, mileage limits)
        public string[] AdditionalServices { get; set; } // (e.g., GPS, child seats, roadside assistance, insurance options)
        public string[] CarFeatures { get; set; }    // Features of the vehicles (e.g., A/C, Bluetooth, navigation, sunroof)
    }
}