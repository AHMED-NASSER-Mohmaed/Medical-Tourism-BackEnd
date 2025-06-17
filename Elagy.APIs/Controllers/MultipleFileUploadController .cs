using Elagy.Core.DTOs.Files;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For IFormFile
using Microsoft.AspNetCore.RateLimiting; // For rate limiting attribute
using System.Collections.Generic;
using System.Linq; // For Any() on IEnumerable and Contains()
using System.Threading.Tasks;
using System; // For generic Exception
using Microsoft.Extensions.Logging; // For ILogger
using Newtonsoft.Json;
using Elagy.Core.Helpers; // For JsonConvert (ensure package is installed in Elagy.APIs if not already)

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Apply the IP-based rate limiting policy to this entire controller.
    // This will limit the number of requests (upload attempts) from a single IP.
    [EnableRateLimiting("IpUploadLimit")]
    public class MediaController : ControllerBase  
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IFileStorageService fileStorageService, ILogger<MediaController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        // Endpoint for clients to upload multiple files.
        // This endpoint does NOT require authentication as it's standalone.
        [HttpPost("upload")]
        // Request body size limit for the entire request (e.g., 20MB total for all files combined)
        [RequestSizeLimit(20 * 1024 * 1024)] // Example: 20 MB total limit for all files in one request
        // You can use [DisableRequestSizeLimit] if you want to bypass the global filter, but if you have a limit here, it's enforced.
        public async Task<ActionResult<MultipleFileUploadResponseDto>> UploadFiles(
            [FromForm] IEnumerable<IFormFile> files,
            [FromForm] string? subFolder = null) // Optional: category like "general"
        {
            if (files == null || !files.Any())
            {
                return BadRequest(new MultipleFileUploadResponseDto { OverallSuccess = false, Message = "No files selected for upload." });
            }

            // Define allowed file types (MIME types) and maximum individual file size
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" }; // Example allowed types
            var maxIndividualFileSizeMB = 5; // Max 5MB per individual file

            // Validate each file in the collection
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    _logger.LogWarning($"Upload attempt rejected: File '{file.FileName}' is empty.");
                    return BadRequest(new MultipleFileUploadResponseDto { OverallSuccess = false, Message = $"File '{file.FileName}' is empty." });
                }
                if (file.Length > maxIndividualFileSizeMB * 1024 * 1024)
                {
                    _logger.LogWarning($"Upload attempt rejected: File '{file.FileName}' exceeds {maxIndividualFileSizeMB}MB individual limit.");
                    return BadRequest(new MultipleFileUploadResponseDto { OverallSuccess = false, Message = $"File '{file.FileName}' exceeds the {maxIndividualFileSizeMB}MB individual limit." });
                }
                if (string.IsNullOrEmpty(file.ContentType) || !allowedMimeTypes.Contains(file.ContentType.ToLower()))
                {
                    _logger.LogWarning($"Upload attempt rejected: File '{file.FileName}' has unsupported type '{file.ContentType}'.");
                    return BadRequest(new MultipleFileUploadResponseDto { OverallSuccess = false, Message = $"File '{file.FileName}' has an unsupported file type: {file.ContentType ?? "unknown"}. Only JPEG, PNG, GIF, PDF are allowed." });
                }
            }

            // Define the subfolder for ImageKit within the temporary storage
            var uploadSubFolder = string.IsNullOrEmpty(subFolder) ? "general" : subFolder;

            // Call the file storage service to upload the files to ImageKit's temporary storage
            var uploadResults = await _fileStorageService.UploadMultipleFilesAsync(files);

            if (!uploadResults.OverallSuccess)
            {
                // Log detailed errors if needed for failed uploads
                _logger.LogError($"Multiple file upload failed overall. Details: {JsonConvert.SerializeObject(uploadResults)}");
                // Return 500 if the server-side upload to ImageKit failed
                return StatusCode(500, uploadResults);
            }

            _logger.LogInformation($"Multiple files uploaded successfully. Overall status: {uploadResults.OverallSuccess}");
            return Ok(uploadResults); // Return ImageKit IDs and URLs to the client
        }



        /// <summary>
        /// Deletes a file from ImageKit.io using its file ID.
        /// </summary>
        /// <param name="imageKitFileId">The unique ID of the file to delete, obtained from a successful upload.</param>
        /// <returns>A status indicating whether the deletion was successful.</returns>
        [HttpDelete("delete/{imageKitFileId}")] // Defines a DELETE endpoint with the file ID as a route parameter
        public async Task<ActionResult> DeleteFile(string imageKitFileId)
        {
            if (string.IsNullOrEmpty(imageKitFileId))
            {
                return BadRequest("ImageKit File ID cannot be empty.");
            }

            try
            {
                bool success = await _fileStorageService.DeleteFileAsync(imageKitFileId);

                if (success)
                {
                    _logger.LogInformation($"API: File deletion request for ID '{imageKitFileId}' processed successfully.");
                    // Return 204 No Content for successful deletion if no body is needed
                    return NoContent();
                    // Or return 200 OK with a success message if you prefer:
                    // return Ok(new { Message = $"File '{imageKitFileId}' deleted successfully." });
                }
                else
                {
                    _logger.LogError($"API: Failed to delete file with ID '{imageKitFileId}' from ImageKit service.");
                    // Return 500 Internal Server Error if the service layer failed
                    return StatusCode(500, "Failed to delete file from ImageKit. Check server logs for details.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: An unexpected error occurred while processing delete request for file ID '{imageKitFileId}'.");
                // Catch any unhandled exceptions and return a 500 error
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }
    


    }
}