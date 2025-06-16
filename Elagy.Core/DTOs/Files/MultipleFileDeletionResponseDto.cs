using System.Collections.Generic;

namespace Elagy.Core.DTOs.Files
{
    public class MultipleFileDeletionResponseDto
    {
        public bool OverallSuccess { get; set; } // True if all deletions succeeded, false if any failed
        public string Message { get; set; } = default!;
        public List<FileDeletionResponseDto> DeletionResults { get; set; } = new List<FileDeletionResponseDto>();
    }
}