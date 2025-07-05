namespace Elagy.Core.Entities
{
        public class HospitalAsset : Asset
        {
                public int NumberOfDepartments { get; set; }
                public bool EmergencyServices { get; set; }
                public ICollection<HospitalSpecialty>? HospitalSpecialties { get; set; }
                public ICollection<HospitalAssetImage>? HospitalAssetImages { get; set; }


    }
}