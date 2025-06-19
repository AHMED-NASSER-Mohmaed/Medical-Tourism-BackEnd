using Elagy.Core.DTOs.Files;
using Microsoft.AspNetCore.Http; // For IFormFile
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elagy.Core.Helpers
{
    public interface IFileStorageService
    {
        public  Task<FileUploadResponseDto> UploadSingleFileAsync(IFormFile file, string? subFolder = null);

        public Task<MultipleFileUploadResponseDto> UploadMultipleFilesAsync(IEnumerable<IFormFile> files, string subFolder = null);

        Task<bool> DeleteFileAsync(string imageKitFileId);
        public  Task<MultipleFileDeletionResponseDto> DeleteMultipleFilesAsync(IEnumerable<string> imageKitFileIds);

    }
}