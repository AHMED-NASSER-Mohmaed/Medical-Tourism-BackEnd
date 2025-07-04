namespace Elagy.Core.Entities
{
    public class ServiceProvider : User
    {

        public DateTime AcquisitionDate { get; set; } = DateTime.Now; // Date of asset acquisition/establishment

        public string NationalURL { get; set; } 
        public string NationalFeildId { get; set; }
        // Navigation property for the one-to-one relationship with ServiceAsset


        // do not foget to add the foreign key for the asset
        public int AssetId{ get; set; }
        public Asset ServiceAsset { get; set; }
 
    }
}