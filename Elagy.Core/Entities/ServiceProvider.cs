namespace Elagy.Core.Entities
{
    public class ServiceProvider : User
    {
        public string NationalURL { get; set; } 
        public string NationalFeildId { get; set; } 
        // Navigation property for the one-to-one relationship with ServiceAsset
        public Asset ServiceAsset { get; set; }
    }
}