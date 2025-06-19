using Elagy.Core.DTOs.User;
using Elagy.Core.Enums;

public class CarRentalProviderProfileDto : BaseServiceProviderProfileDto
{
    public Governorate[] OperationalAreas { get; set; } // Geographic areas where car rental operates

    public FuelType[] FuelTypes { get; set; }

    //Ai generated
    public string[] Models { get; set; }  // Specific models available for rent (e.g., Toyota Camry, Ford Explorer)

    public TransmissionType Transmission { get; set; }   // "Automatic", "Manual"
    public string[] RentalPolicies { get; set; } // (e.g., age restrictions, insurance requirements, mileage limits)
}