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

        //payment information
        public Disbursement Disbursement { get; set; } // Payment information related to the service provider
        public string DisbursementId { get; set; } // Foreign key for Disbursement

    }
}