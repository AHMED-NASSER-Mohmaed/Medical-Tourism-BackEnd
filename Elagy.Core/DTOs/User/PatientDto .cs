namespace Elagy.Core.DTOs.User
{
    public class PatientDto : BaseProfileDto
    {
        public string BloodGroup { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
    }
}