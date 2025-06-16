using System.Collections.Generic;

namespace Elagy.Core.DTOs.Files
{
 
    public class MultipleFileUploadResponseDto
    {
        public bool OverallSuccess { get; set; }
        public string Message { get; set; } = "";
        public List<FileUploadResponseDto> UploadResults { get; set; } = new List<FileUploadResponseDto>();
    }
}