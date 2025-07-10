using Elagy.Core.DTOs.Files;
using Elagy.Core.Helpers;
using Elagy.Core.Temps;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Helpers
{
    public class ImageKitFileStorageService : IFileStorageService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageKitFileStorageService> _logger;
        private readonly ImageKitSettings _imageKitSettings; 


        public ImageKitFileStorageService(
            HttpClient httpClient,
            IOptions<ImageKitSettings> imageKitOptions,  
            ILogger<ImageKitFileStorageService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _imageKitSettings = imageKitOptions.Value; // NEW: Get the value from IOptions

            // Now use properties from _imageKitSettings
            if (string.IsNullOrEmpty(_imageKitSettings.PrivateKey) || string.IsNullOrEmpty(_imageKitSettings.PublicKey) || string.IsNullOrEmpty(_imageKitSettings.BaseUrl))
            {
                throw new ArgumentNullException("ImageKit settings (PublicKey, PrivateKey, or BaseUrl) are not configured correctly.");
            }

            // Set default Authorization header for this HttpClient instance using _imageKitSettings.PrivateKey
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_imageKitSettings.PrivateKey}:")));
        }
        /// <summary>
        /// Uploads a single file to ImageKit.io's temporary storage, handling authentication and response mapping.
        /// </summary>
        public async Task<FileUploadResponseDto> UploadSingleFileAsync(IFormFile file, string? subFolder = null)
        {
            _logger.LogInformation($"Attempting to upload file: {file.FileName} to subfolder: {subFolder ?? " (none provided)"}");

            // Initialize a response DTO for this single file upload
            var uploadResponseDto = new FileUploadResponseDto
            {
                Success = false, // Assume failure until proven otherwise
                FileName = file.FileName,
                Message = "Upload failed due to an unknown error." // Default error message
            };

            try
            {
                // 1. Prepare Authentication Header: ImageKit requires Basic Auth with "PrivateKey:" Base64 encoded.
                string privateKeyWithColon = _imageKitSettings.PrivateKey + ":"; // CRUCIAL: Add the colon
                string base64AuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKeyWithColon));

                // 2. Prepare the Form Data for the Request
                using (var content = new MultipartFormDataContent())
                {
                    // Add the file stream
                    // Use a using statement to ensure the stream is disposed after reading.
                    using (var fileStream = file.OpenReadStream())
                    {
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        // "file" is the required parameter name for ImageKit
                        content.Add(streamContent, "file", file.FileName);

                        // --- ADDED: Explicitly add the 'fileName' parameter as a string content ---
                        // ImageKit explicitly requires a 'fileName' parameter in the form data
                        content.Add(new StringContent(file.FileName), "fileName");  

                        // Determine the target folder in ImageKit
                        // ImageKit's API expects the 'folder' parameter to be the full path.
                        // If subFolder is provided, combine it with the base UploadFolderPath.
                        var targetFolder = string.IsNullOrEmpty(subFolder)
                            ? _imageKitSettings.UploadFolderPath // Use base folder if no subFolder provided
                            : Path.Combine(_imageKitSettings.UploadFolderPath, subFolder).Replace("\\", "/"); // Combine and ensure forward slashes

                        // Add the folder parameter
                        // "folder" is the required parameter name for ImageKit
                        content.Add(new StringContent(targetFolder), "folder");

                        // Optional: Request ImageKit to use a unique file name
                        content.Add(new StringContent("true"), "useUniqueFileName");

                        // Optional: Tags can be useful for organizing files in ImageKit
                        // content.Add(new StringContent("my-app,user-uploads"), "tags");

                        // 3. Create the HttpRequestMessage
                        // Use the dedicated UploadApiUrl from settings
                        var request = new HttpRequestMessage(HttpMethod.Post, _imageKitSettings.UploadApiUrl);

                        // Add the Authorization header to THIS SPECIFIC request
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64AuthString);

                        request.Content = content; // Attach the form data content

                        // Log request details for debugging
                        _logger.LogDebug($"Sending ImageKit upload request to: {_imageKitSettings.UploadApiUrl}");
                        _logger.LogDebug($"Authorization Header: Basic {base64AuthString}");
                        _logger.LogDebug($"File Name: {file.FileName}, Target Folder: {targetFolder}");

                        // 4. Send the Request
                        var httpResponse = await _httpClient.SendAsync(request);

                        // 5. Handle the Response
                        string responseBody = await httpResponse.Content.ReadAsStringAsync();
                        _logger.LogInformation($"ImageKit API Response Status: {httpResponse.StatusCode} ({httpResponse.ReasonPhrase})");
                        _logger.LogInformation($"ImageKit API Response Body: {responseBody}");

                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // ImageKit successful response returns JSON with fileId, url, etc.
                            // Deserialize to a temporary object that matches ImageKit's success response structure
                            var imageKitSuccessResponse = JsonConvert.DeserializeObject<ImageKitUploadSuccessApiDto>(responseBody);

                            if (imageKitSuccessResponse != null)
                            {
                                // Map ImageKit's response to your FileUploadResponseDto
                                uploadResponseDto.Success = true;
                                uploadResponseDto.Id = imageKitSuccessResponse.fileId; // Map to your 'Id'
                                uploadResponseDto.FileName = imageKitSuccessResponse.name;
                                uploadResponseDto.Url = imageKitSuccessResponse.url;
                                uploadResponseDto.FileType = imageKitSuccessResponse.fileType;
                                uploadResponseDto.Size = imageKitSuccessResponse.size;
                                uploadResponseDto.ThumbnailUrl = imageKitSuccessResponse.thumbnailUrl;
                                uploadResponseDto.Message = $"File '{file.FileName}' uploaded successfully to ImageKit.";
                                uploadResponseDto.Error = null; // Clear any previous error
                            }
                            else
                            {
                                uploadResponseDto.Message = $"Failed to parse ImageKit success response for '{file.FileName}'. Raw: {responseBody}";
                                _logger.LogError(uploadResponseDto.Message);
                            }
                        }
                        else
                        {
                            // ImageKit returns an error, try to parse it if it's JSON
                            // For 403, the content type was text/html, so direct JSON parsing might fail.
                            // Log the raw body for manual inspection.
                            uploadResponseDto.Success = false;
                            uploadResponseDto.Message = $"ImageKit upload failed for '{file.FileName}'. Status: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}.";
                            uploadResponseDto.Error = responseBody; // Store the full error body for debugging
                            _logger.LogError($"ImageKit API error: {uploadResponseDto.Message}. Response Body: {responseBody}");
                        }
                    } // fileStream disposed
                } // content disposed
            }
            catch (HttpRequestException httpEx)
            {
                // Network-level errors (e.g., DNS resolution, connection refused, SSL errors)
                uploadResponseDto.Message = $"Network/HTTP error during ImageKit upload for '{file.FileName}': {httpEx.Message}";
                uploadResponseDto.Error = httpEx.ToString(); // Include full exception details
                _logger.LogError(httpEx, uploadResponseDto.Message);
            }
            catch (JsonException jsonEx)
            {
                // Error deserializing ImageKit's response (e.g., unexpected format)
                uploadResponseDto.Message = $"Error parsing ImageKit response for '{file.FileName}': {jsonEx.Message}";
                uploadResponseDto.Error = jsonEx.ToString();
                _logger.LogError(jsonEx, uploadResponseDto.Message);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                uploadResponseDto.Message = $"An unexpected error occurred during ImageKit upload for '{file.FileName}': {ex.Message}";
                uploadResponseDto.Error = ex.ToString();
                _logger.LogError(ex, uploadResponseDto.Message);
            }

            return uploadResponseDto;
        }
        /// <summary>
        /// Uploads multiple files to ImageKit.io's temporary storage concurrently.
        /// </summary>
        public async Task<MultipleFileUploadResponseDto> UploadMultipleFilesAsync(IEnumerable<IFormFile> files, string subFolder = null)
        {
            var response = new MultipleFileUploadResponseDto { OverallSuccess = true, Message = "All files processed." };
            var uploadTasks = new List<Task<FileUploadResponseDto>>();

            /*if (files == null || !files.Any())
            {
                response.OverallSuccess = false;
                response.Message = "No files provided for upload.";
                return response;
            }*/

            foreach (var file in files)
            {
                uploadTasks.Add(UploadSingleFileAsync(file, subFolder));
            }

            var results = await Task.WhenAll(uploadTasks);

            foreach (var result in results)
            {
                response.UploadResults.Add(result);
                if (!result.Success)
                {
                    response.OverallSuccess = false;
                    response.Message = "Some files failed to upload to ImageKit.";
                }
            }
            return response;
        }


        /// <summary>
        /// Deletes a file from ImageKit.io by its ImageKit fileId.
        /// (Included as part of IFileStorageService, but not directly used by this standalone controller's flow)
        /// </summary>
        public async Task<bool> DeleteFileAsync(string imageKitFileId)
        {
            // Internal method, we'll now have DeleteMultipleFilesAsync call a version that returns FileDeletionResponseDto
            // Let's modify this to use the more detailed DTO for consistency
            var result = await DeleteSingleFileInternalAsync(imageKitFileId);
            return result.Success;
        }

        private async Task<FileDeletionResponseDto> DeleteSingleFileInternalAsync(string imageKitFileId)
        {
            var deletionResult = new FileDeletionResponseDto
            {
                ImageKitFileId = imageKitFileId,
                Success = false,
                Message = $"Failed to delete file '{imageKitFileId}'."
            };

            if (string.IsNullOrEmpty(imageKitFileId))
            {
                deletionResult.Message = "File ID cannot be empty.";
                deletionResult.ErrorDetails = "Empty file ID provided for deletion.";
                _logger.LogWarning(deletionResult.Message);
                return deletionResult;
            }

            try
            {
                // 1. Prepare Authentication Header
                string privateKeyWithColon = _imageKitSettings.PrivateKey + ":";
                string base64AuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKeyWithColon));

                // 2. Create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{_imageKitSettings.ManageApiUrl}/{imageKitFileId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64AuthString);

                _logger.LogInformation($"Attempting to delete file with ID: '{imageKitFileId}' from ImageKit.");
                _logger.LogDebug($"DELETE request URL: {request.RequestUri}");

                var response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync(); // Always read response body for detailed logging

                if (response.IsSuccessStatusCode)
                {
                    deletionResult.Success = true;
                    deletionResult.Message = $"File with ID '{imageKitFileId}' deleted successfully from ImageKit.";
                    _logger.LogInformation(deletionResult.Message);
                }
                else
                {
                    deletionResult.Message = $"ImageKit deletion failed for '{imageKitFileId}'. Status: {(int)response.StatusCode} {response.ReasonPhrase}.";
                    deletionResult.ErrorDetails = responseBody; // Store the full error body for debugging
                    _logger.LogError(deletionResult.Message + $" Response: {responseBody}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                deletionResult.Message = $"Network/HTTP error during ImageKit deletion for '{imageKitFileId}': {httpEx.Message}";
                deletionResult.ErrorDetails = httpEx.ToString();
                _logger.LogError(httpEx, deletionResult.Message);
            }
            catch (Exception ex)
            {
                deletionResult.Message = $"An unexpected error occurred during ImageKit deletion for '{imageKitFileId}': {ex.Message}";
                deletionResult.ErrorDetails = ex.ToString();
                _logger.LogError(ex, deletionResult.Message);
            }

            return deletionResult;
        }


        /// <summary>
        /// Deletes multiple files from ImageKit.io concurrently using their file IDs.
        /// </summary>
        public async Task<MultipleFileDeletionResponseDto> DeleteMultipleFilesAsync(IEnumerable<string> imageKitFileIds)
        {
            var response = new MultipleFileDeletionResponseDto { OverallSuccess = true, Message = "All files processed." };
            var deletionTasks = new List<Task<FileDeletionResponseDto>>();

            if (imageKitFileIds == null || !imageKitFileIds.Any())
            {
                response.OverallSuccess = false;
                response.Message = "No file IDs provided for deletion.";
                return response;
            }

            foreach (var fileId in imageKitFileIds)
            {
                // Call the internal single file deletion method which returns the detailed DTO
                deletionTasks.Add(DeleteSingleFileInternalAsync(fileId));
            }

            var results = await Task.WhenAll(deletionTasks);

            foreach (var result in results)
            {
                response.DeletionResults.Add(result);
                if (!result.Success)
                {
                    response.OverallSuccess = false;
                    response.Message = "Some files failed to delete from ImageKit."; // General message for overall failure
                }
            }

            if (response.OverallSuccess)
            {
                response.Message = $"Successfully deleted {response.DeletionResults.Count(r => r.Success)} out of {response.DeletionResults.Count} files.";
            }
            else
            {
                int successfulDeletions = response.DeletionResults.Count(r => r.Success);
                int failedDeletions = response.DeletionResults.Count(r => !r.Success);
                response.Message = $"Completed deletion attempts. {successfulDeletions} files deleted, {failedDeletions} files failed.";
            }

            return response;
        }


        // Helper method to convert IFormFile to byte array for uploading
        private async Task<byte[]> GetBytesAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }

}