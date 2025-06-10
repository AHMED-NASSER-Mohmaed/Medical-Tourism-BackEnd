using Elagy.Core.DTOs.User;

public class CarRentalProviderProfileDto : BaseServiceProviderProfileDto
{
    public string OperationalAreas { get; set; }
    public string[] VehicleType { get; set; }
    public string[] Transmission { get; set; }
    public string[] FuelType { get; set; }
    public string[] RentalPolicies { get; set; }
    public string[] AdditionalServices { get; set; }
    public string[] CarFeatures { get; set; }
}