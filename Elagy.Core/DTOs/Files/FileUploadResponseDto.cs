namespace Elagy.Core.DTOs.Files
{
    public class FileUploadResponseDto
    {
        public bool Success { get; set; }
        public string Id { get; set; } // ImageKit fileId
        public string FileName { get; set; } // Original file name (as received)
        public string Url { get; set; } // ImageKit URL (publicly accessible)
        public string FileType { get; set; } // e.g., "image", "raw", "video"
        public long Size { get; set; } // File size in bytes
        public string ThumbnailUrl { get; set; } // If applicable for images
        public string Message { get; set; }
        public string Error { get; set; } // Error message if upload failed
    }
}