using Elagy.Core.DTOs.User;

public class HospitalProviderProfileDto : BaseServiceProviderProfileDto
{
    public int NumberOfDepartments { get; set; }
    public bool EmergencyServices { get; set; }

    public ICollection<AssetImageResponseDto>? AssetImages { get; set; }
}
