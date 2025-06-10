namespace Elagy.Core.Entities
{
    public class ServiceProvider : User
    {
        public string DOCsURL { get; set; } // Documents for the ServiceProvider company/individual
        // Navigation property for the one-to-one relationship with ServiceAsset
        public ServiceAsset ServiceAsset { get; set; }
    }
}