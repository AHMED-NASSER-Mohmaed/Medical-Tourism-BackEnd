using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Files
{
    public class ImageKitUploadSuccessApiDto
    {
        public string fileId { get; set; } = default!;
        public string name { get; set; } = default!;
        public string url { get; set; } = default!;
        public string thumbnailUrl { get; set; } = default!;
        public string fileType { get; set; } = default!; // Matches your FileType
        public long size { get; set; } // Matches your Size
        // You can add other properties from ImageKit's response if you need them:
        // public int height { get; set; }
        // public int width { get; set; }
        // public string AITags { get; set; }
        // etc.
    }
}
