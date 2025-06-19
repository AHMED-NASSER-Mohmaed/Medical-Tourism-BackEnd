using Elagy.Core.DTOs.Files;
using Elagy.Core.Entities;
using Elagy.Core.Helpers;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elagy.BL.Services
{
    public class ImageProfile : IImageProfile
    {
        private readonly UserManager<User> _userManager;
        private readonly IFileStorageService _fileStorageService; // Inject your ImageKit service
        private readonly ILogger<ImageProfile> _logger;

        public ImageProfile(UserManager<User> userManager, IFileStorageService fileStorageService, ILogger<ImageProfile> logger)
        {
            _userManager = userManager;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }


        /// <summary>
        /// Deletes a user's profile image from ImageKit and clears its URL/ID from the user's record.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile image is to be deleted.</param>
        /// <returns>A FileDeletionResponseDto indicating the outcome of the operation.</returns>
        public async Task<FileDeletionResponseDto> DeleteImageProfile(string userId)
        {
            var response = new FileDeletionResponseDto
            {
                ImageKitFileId = "", // Will be updated if a file ID is found
                Success = false,
                Message = "Operation failed due to an unknown error."
            };

            if (string.IsNullOrEmpty(userId))
            {
                response.Message = "User ID cannot be empty.";
                _logger.LogWarning($"DeleteImageProfile: User ID is null or empty.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Message = "User not found.";
                _logger.LogWarning($"DeleteImageProfile: User with ID '{userId}' not found.");
                return response;
            }

            // Assuming your User entity has ImageId and ImageURL properties for the profile image.
            // If not, you might need to adapt this to find the image ID from another related entity (e.g., ServiceAsset).
            if (string.IsNullOrEmpty(user.ImageId))
            {
                response.Message = "User does not have a profile image ID stored.";
                response.Success = true; // Consider this a success if there's nothing to delete
                _logger.LogInformation($"DeleteImageProfile: User '{user.Email}' (ID: '{userId}') does not have a profile image ID. Nothing to delete.");
                return response;
            }

            response.ImageKitFileId = user.ImageId; // Store the ID for the response

            _logger.LogInformation($"DeleteImageProfile: Attempting to delete profile image '{user.ImageURL}' (ID: '{user.ImageId}') for user '{user.Email}'.");

            try
            {
                // Call your IFileStorageService to delete the file from ImageKit
                var deleteResult = await _fileStorageService.DeleteFileAsync(user.ImageId);

                if (deleteResult) // Assuming DeleteFileAsync returns bool
                {
                    // Clear the image details from the user's database record
                    user.ImageId = null;
                    user.ImageURL = null; // Clear the URL as well
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        response.Success = true;
                        response.Message = $"Profile image '{response.ImageKitFileId}' deleted successfully for user '{user.Email}'.";
                        _logger.LogInformation(response.Message);
                    }
                    else
                    {
                        response.Message = $"Profile image deleted from ImageKit, but failed to update user record: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}.";
                        response.ErrorDetails = "User record update failed.";
                        _logger.LogError(response.Message);
                    }
                }
                else
                {
                    // If DeleteFileAsync returns false, it means ImageKit deletion failed for some reason
                    response.Message = $"Failed to delete profile image '{response.ImageKitFileId}' from ImageKit.";
                    // You might need more granular error info from DeleteFileAsync if it had more details
                    _logger.LogError(response.Message);
                }
            }
            catch (Exception ex)
            {
                response.Message = $"An unexpected error occurred while deleting profile image '{response.ImageKitFileId}' for user '{user.Email}'.";
                response.ErrorDetails = ex.Message;
                _logger.LogError(ex, response.Message);
            }

            return response;
        }



        /// <summary>
        /// Updates a user's profile image. Deletes the old image from ImageKit (if exists),
        /// uploads the new one, and updates the user's database record.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile image is being updated.</param>
        /// <param name="newProfileImage">The new image file (IFormFile).</param>
        /// <returns>ImageKitUploadSuccessApiDto if successful, or throws an InvalidOperationException otherwise.</returns>
        public async Task<FileUploadResponseDto> UpdateImageProfile(string userId, IFormFile newProfileImage)
        {
            _logger.LogInformation($"UpdateImageProfile: Attempting to update profile image for user ID: '{userId}'.");

            // --- Input Validation ---
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UpdateImageProfile: User ID is null or empty.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (newProfileImage == null || newProfileImage.Length == 0)
            {
                _logger.LogWarning("UpdateImageProfile: No new profile image file provided.");
                throw new ArgumentException("No profile image file provided.", nameof(newProfileImage));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"UpdateImageProfile: User with ID '{userId}' not found.");
                throw new InvalidOperationException($"User with ID '{userId}' not found.");
            }

            string oldImageId = user.ImageId; // Assuming User entity has ImageId property
            string oldImageUrl = user.ImageURL; // Assuming User entity has ImageURL property

            // --- Step 1: Delete Old Image (if it exists) ---
            if (!string.IsNullOrEmpty(oldImageId))
            {
                _logger.LogInformation($"UpdateImageProfile: Deleting old profile image '{oldImageUrl}' (ID: '{oldImageId}') for user '{user.Email}'.");
                var deleteSuccess = await _fileStorageService.DeleteFileAsync(oldImageId);

                if (!deleteSuccess)
                {
                    // Log a warning but proceed with the new upload.
                    // You might choose to throw an exception here if deleting the old image is critical for your app.
                    _logger.LogWarning($"UpdateImageProfile: Failed to delete old profile image '{oldImageId}' from ImageKit. Proceeding with new upload.");
                }
            }

            // --- Step 2: Upload New Image ---
            _logger.LogInformation($"UpdateImageProfile: Uploading new profile image '{newProfileImage.FileName}' for user '{user.Email}'.");
            // Assuming 'profile-images' is a suitable subfolder in ImageKit for user profiles
            FileUploadResponseDto uploadResult = await _fileStorageService.UploadSingleFileAsync(newProfileImage, "profile-images");

            if (!uploadResult.Success)
            {
                _logger.LogError($"UpdateImageProfile: Failed to upload new profile image for user '{user.Email}'. Details: {uploadResult.Message}");

                return uploadResult;
            }

            // --- Step 3: Update User's Profile Image Details in Database ---
            user.ImageId = uploadResult.Id;
            user.ImageURL = uploadResult.Url;
            //user.ImageThumbnailURL = uploadResult.ThumbnailUrl; // Update thumbnail URL if your User entity has this property

            _logger.LogInformation($"UpdateImageProfile: Updating user record with new image details for '{user.Email}'. New ID: '{user.ImageId}', URL: '{user.ImageURL}'.");
            var updateIdentityUserResult = await _userManager.UpdateAsync(user);

            if (!updateIdentityUserResult.Succeeded)
            {
                // If updating the user record in the database fails after successful upload to ImageKit,
                // attempt to delete the newly uploaded image from ImageKit to prevent orphaned files.
                _logger.LogError($"UpdateImageProfile: Failed to update user record for '{user.Email}' after successful image upload. Errors: {string.Join(", ", updateIdentityUserResult.Errors.Select(e => e.Description))}. Attempting to rollback new image upload from ImageKit.");

                // It's important to check if FileId is not null before attempting deletion
                /*if (!string.IsNullOrEmpty(uploadResult.Id))
                {
                    //deleting process is not neccessarly
                    await _fileStorageService.DeleteFileAsync(uploadResult.Id); // Rollback new upload

                    _logger.LogWarning($"UpdateImageProfile: Rolled back newly uploaded image '{uploadResult.Id}' from ImageKit due to user record update failure.");
                }*/

                var message = $"Failed to update user profile image in database: {string.Join(", ", updateIdentityUserResult.Errors.Select(e => e.Description))}";

                return new FileUploadResponseDto {Success=false,Error= message};  
            }
            _logger.LogInformation($"UpdateImageProfile: Profile image updated successfully for user '{user.Email}'.");
            return uploadResult;
        }



    }
}
