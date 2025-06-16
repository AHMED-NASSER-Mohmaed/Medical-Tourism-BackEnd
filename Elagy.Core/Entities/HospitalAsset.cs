namespace Elagy.Core.Entities
{
    public class HospitalAsset : ServiceAsset
    {
        // Navigation properties for medical specialties/departments can be added here
        // e.g., public ICollection<MedicalSpecialty> MedicalSpecialties { get; set; }

        public int NumberOfDepartments { get; set; }
        public bool HasEmergencyRoom { get; set; }
        public bool IsTeachingHospital { get; set; }
        public bool EmergencyServices { get; set; } // Indicates if 24/7 emergency services are available
        public ICollection<HospitalSpecialty> HospitalSpecialties { get; set; }
    }
}