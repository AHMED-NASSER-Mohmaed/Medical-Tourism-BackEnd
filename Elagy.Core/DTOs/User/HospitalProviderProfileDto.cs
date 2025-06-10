using Elagy.Core.DTOs.User;

public class HospitalProviderProfileDto : BaseServiceProviderProfileDto
{
    public int NumberOfDepartments { get; set; }
    public bool HasEmergencyRoom { get; set; }
    public bool IsTeachingHospital { get; set; }
    public bool EmergencyServices { get; set; }
}
