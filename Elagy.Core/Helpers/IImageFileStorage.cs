using Elagy.Core.DTOs.Files;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elagy.Core.Helpers
{
    public interface IImageFileStorage
    {
        /// <summary>
        /// Uploads multiple files in parallel to the storage service.
        /// </summary>
        /// <param name="files">A list of byte arrays representing the file contents.</param>
        /// <param name="fileNames">A list of original file names, corresponding to the 'files' list.</param>
        /// <param name="paths">Optional: A list of destination paths/folders for each file. If null, a default path will be used.</param>
        /// <returns>A list of URLs or identifiers for the uploaded files.</returns>
        Task<List<string>> UploadMultipleFilesParallelAsync(List<byte[]> files, List<string> fileNames, List<string> paths = null);

        /// <summary>
        /// Deletes multiple files in parallel from the storage service.
        /// </summary>
        /// <param name="fileIdentifiers">A list of URLs or identifiers (ImageKit fileIds) of the files to be deleted.</param>
        /// <returns>A boolean indicating if all deletions were successful.</returns>
        Task<bool> DeleteMultipleFilesParallelAsync(List<string> fileIdentifiers);


        Task<MultipleFileDeletionResponseDto> DeleteMultipleFilesAsync(IEnumerable<string> imageKitFileIds);

    }
}