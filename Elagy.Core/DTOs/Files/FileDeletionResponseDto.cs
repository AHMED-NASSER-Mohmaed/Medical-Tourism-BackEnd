using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Files
{
    public class FileDeletionResponseDto
    {
        public bool Success { get; set; }
        public string ImageKitFileId { get; set; } = default!; // The ID of the file attempted to delete
        public string Message { get; set; } = default!; // Success or error message for this specific file
        public string? ErrorDetails { get; set; } // Optional: More detailed error info
    }


}
