namespace Elagy.Core.Temps
{
    public class ImageKitSettings
    {
        public const string ImageKitSectionName = "ImageKitSettings";

        public string PublicKey { get; set; } = default!;
        public string PrivateKey { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string UploadFolderPath { get; set; } = default!;

        // ADD THIS NEW PROPERTY:
        public string UploadApiUrl { get; set; } = default!; // e.g., "https://upload.imagekit.io/api/v1/files/upload"

        // Although not used in your current snippet, if you have a Manage API URL, it's good to keep it:
         public string ManageApiUrl { get; set; } = default!;
    }
}