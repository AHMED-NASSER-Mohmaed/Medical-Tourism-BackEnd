using System;

namespace Elagy.Core.Entities
{
    public class ImageKitTempFile
    {
        public string Id { get; set; } // ImageKit fileId (unique identifier from ImageKit)
        public string OriginalFileName { get; set; }
        public string ImageKitUrl { get; set; } // The temporary URL where it was uploaded
        public string ImageKitFilePath { get; set; } // The path within ImageKit (e.g., /elagy-uploads/temp/...)
        public DateTime UploadedDate { get; set; }
        public bool IsConfirmed { get; set; } // True if associated with a successful registration/update
    }
}