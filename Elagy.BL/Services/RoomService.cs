using AutoMapper;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Room;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Elagy.BL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IFileStorageService _fileStorageService;

        public RoomService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RoomService> logger,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }


        public async Task<RoomResponseDto> ChangeRoomAvailabilityStatusAsync(int roomId, bool newIsAvailableStatus, string hotelAssetId)
        {
            try
            {
                var room = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (room == null) throw new KeyNotFoundException($"Room with ID {roomId} not found for availability status change.");

                if (room.HotelAssetId != hotelAssetId) throw new UnauthorizedAccessException($"Room with ID {roomId} is not affiliated with your hotel.");

                if (room.IsAvailable == newIsAvailableStatus)
                {
                    _logger.LogInformation($"Room with ID {roomId} is already in the target IsAvailable status ({newIsAvailableStatus}). No action needed.");
                    return _mapper.Map<RoomResponseDto>(room);
                }

                room.IsAvailable = newIsAvailableStatus;
                _unitOfWork.Rooms.Update(room); 
                await _unitOfWork.CompleteAsync(); 

                _logger.LogInformation($"Room '{roomId}' IsAvailable status changed to {newIsAvailableStatus} successfully for Hotel {hotelAssetId}.");


                var updatedRoomWithDetails = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (updatedRoomWithDetails == null) throw new Exception("Updated room not found after save.");

                return _mapper.Map<RoomResponseDto>(updatedRoomWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing availability status for room ID: {roomId} for Hotel {hotelAssetId}.");
                throw;
            }
        }

        public async Task<RoomResponseDto> ChangeRoomInternalStatusAsync(int roomId, RoomStatus newInternalStatus, string hotelAssetId)
        {
            try
            {

                var room = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (room == null) throw new KeyNotFoundException($"Room with ID {roomId} not found for internal status change.");


                if (room.HotelAssetId != hotelAssetId) throw new UnauthorizedAccessException($"Room with ID {roomId} is not affiliated with your hotel.");

      
                if (room.Status == newInternalStatus)
                {
                    _logger.LogInformation($"Room with ID {roomId} is already in the target internal status ({newInternalStatus}). No action needed.");
                    return _mapper.Map<RoomResponseDto>(room);
                }

                // Perform additional validation based on status change(for booking).



                room.Status = newInternalStatus;
                _unitOfWork.Rooms.Update(room); // Mark as modified
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Room '{roomId}' internal status changed to {newInternalStatus} successfully for Hotel {hotelAssetId}.");

                
                var updatedRoomWithDetails = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (updatedRoomWithDetails == null) throw new Exception("Updated room not found after save.");

                return _mapper.Map<RoomResponseDto>(updatedRoomWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing internal status for room ID: {roomId} for Hotel {hotelAssetId}.");
                throw;
            }
        }

        public async Task<RoomResponseDto> CreateRoomAsync(RoomCreateDto createDto, string hotelAssetId, List<IFormFile>? imageFiles = null)
        {
            try
            {

                var hotelAsset = await _unitOfWork.HotelAssets.GetByIdAsync(hotelAssetId);
                if (hotelAsset == null) throw new ArgumentException($"Hotel Asset with ID {hotelAssetId} not found.");


                var existingRoom = await _unitOfWork.Rooms.FindAsync(r => r.HotelAssetId == hotelAssetId && r.RoomNumber.ToLower() == createDto.RoomNumber.ToLower());
                if (existingRoom.Any()) throw new InvalidOperationException($"Room number '{createDto.RoomNumber}' already exists in this hotel.");


                var room = _mapper.Map<Room>(createDto);
                room.HotelAssetId = hotelAssetId;
                room.IsAvailable = true; 


                if (imageFiles != null && imageFiles.Any())
                {
                    var uploadResult = await _fileStorageService.UploadMultipleFilesAsync(imageFiles);
                    if (uploadResult.OverallSuccess)
                    {
                        room.RoomImages = uploadResult.UploadResults.Where(r => r.Success)
                            .Select((uploadedFile, index) => new RoomImage
                            {
                                ImageId = uploadedFile.Id,
                                ImageURL = uploadedFile.Url,
                                RoomId = room.Id
                            }).ToList();
                        _logger.LogInformation($"Successfully uploaded {room.RoomImages.Count} images for new room.");
                    }
                    else
                    {
                        _logger.LogError($"Some or all images failed to upload for new room '{createDto.RoomNumber}'. Details: {uploadResult.Message}");
                        room.RoomImages = new List<RoomImage>();
                    }

                }
                else
                {
                    room.RoomImages = new List<RoomImage>();
                }


                await _unitOfWork.Rooms.AddAsync(room);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Room '{room.RoomNumber}' (ID: {room.Id}) created successfully for Hotel {hotelAssetId}.");

               
                var createdRoomWithDetails = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(room.Id);
                if (createdRoomWithDetails == null) throw new Exception("Created room not found after save and detail fetch.");

                return _mapper.Map<RoomResponseDto>(createdRoomWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating room for Hotel {hotelAssetId}.");
                throw;
            }
        }

        public async Task<PagedResponseDto<RoomResponseDto>> GetAllRoomsForHotelAdminAsync(string hotelAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                IQueryable<Room> query = _unitOfWork.Rooms.AsQueryable();

                query = query.Where(r => r.HotelAssetId == hotelAssetId);

                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(r => r.RoomNumber.Contains(term));

                }
                if (paginationParameters.RoomType.HasValue)
                {
                    query = query.Where(r => r.RoomType == paginationParameters.RoomType.Value);
                }
                if (paginationParameters.RoomStatus.HasValue)
                {
                    query = query.Where(r => r.Status == paginationParameters.RoomStatus.Value);
                }
                if (paginationParameters.FilterIsActive.HasValue)
                {
                    query = query.Where(r => r.IsAvailable == paginationParameters.FilterIsActive.Value);
                }

                if (paginationParameters.MinPrice.HasValue)
                {
                    query = query.Where(r => r.Price >= paginationParameters.MinPrice.Value);
                }
                if (paginationParameters.MaxPrice.HasValue)
                {
                    query = query.Where(r => r.Price <= paginationParameters.MaxPrice.Value);
                }
                var totalCount = await query.CountAsync();

                var pagedRooms = await query
                        .Include(r => r.HotelAsset)
                            .ThenInclude(ha => ha.ServiceProvider)
                                .ThenInclude(sp => sp.Governorate)
                                    .ThenInclude(g => g.Country)
                        .Include(r => r.RoomImages)
                        .OrderBy(r => r.RoomNumber)
                        .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                        .Take(paginationParameters.PageSize)
                        .ToListAsync();

                var roomDtos = _mapper.Map<IEnumerable<RoomResponseDto>>(pagedRooms);
                return new PagedResponseDto<RoomResponseDto>(roomDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all rooms for Hotel Admin Dashboard for Hotel ID: {hotelAssetId}.");
                return new PagedResponseDto<RoomResponseDto>(Enumerable.Empty<RoomResponseDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }

          }

        public async Task<PagedResponseDto<RoomResponseDto>> GetAvailableRoomsForWebsiteAsync(
    PaginationParameters paginationParameters, string hotelId)
        {
            try
            {
                // 1. Get all rooms of the hotel (with full details)
                var allRooms = await _unitOfWork.Rooms.GetRoomsByHotelId(hotelId);

                // 2. Convert to IQueryable for in-memory filtering
                var query = allRooms.AsQueryable();

                // 3. Filter only available and properly statused rooms
                query = query.Where(r => r.IsAvailable && (r.Status!= RoomStatus.UnderMaintenance));

                // 4. Optional: filter by HotelAssetId again if passed
                if (!string.IsNullOrWhiteSpace(paginationParameters.HotelAssetId))
                    query = query.Where(r => r.HotelAssetId == paginationParameters.HotelAssetId);

                if (paginationParameters.RoomType.HasValue)
                    query = query.Where(r => r.RoomType == paginationParameters.RoomType.Value);

                if (paginationParameters.MinPrice.HasValue)
                    query = query.Where(r => r.Price >= paginationParameters.MinPrice.Value);

                if (paginationParameters.MaxPrice.HasValue)
                    query = query.Where(r => r.Price <= paginationParameters.MaxPrice.Value);

                // Governorate filter via nested navigation (ensure navigation loaded in repository)
                if (paginationParameters.FilterGovernorateId.HasValue)
                {
                    query = query.Where(r =>
                        r.HotelAsset != null &&
                        r.HotelAsset.ServiceProvider != null &&
                        r.HotelAsset.ServiceProvider.GovernorateId == paginationParameters.FilterGovernorateId.Value);
                }

                // 5. Count and paginate
                var totalCount = query.Count();

                var pagedRooms = query
                    .OrderBy(r => r.Price)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                // 6. Map to DTO
                var roomDtos = _mapper.Map<IEnumerable<RoomResponseDto>>(pagedRooms);

                return new PagedResponseDto<RoomResponseDto>(
                    roomDtos,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available rooms for website.");
                return new PagedResponseDto<RoomResponseDto>(
                    Enumerable.Empty<RoomResponseDto>(),
                    0,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );
            }
        }


        public async Task<RoomResponseDto?> GetRoomByIdAsync(int roomId)
        {
            try
            {
                var room = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (room == null)
                {
                    _logger.LogInformation($"Room with ID {roomId} not found.");
                    return null;
                }
                return _mapper.Map<RoomResponseDto>(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting room by ID: {roomId}.");
                throw;
            }
        }

        public async Task<RoomResponseDto> UpdateRoomAsync(int roomId, RoomUpdateDto updateDto, string hotelAssetId, List<IFormFile>? newImageFiles = null)
        {
            try
            {
                // 1. Retrieve the existing room with details (including current RoomImages).
                var room = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (room == null) throw new KeyNotFoundException($"Room with ID {roomId} not found for update.");

                // 2. Validate room's affiliation to the requesting hotel admin.
                if (room.HotelAssetId != hotelAssetId) throw new UnauthorizedAccessException($"Room with ID {roomId} is not affiliated with your hotel.");

                // 3. Validate and handle specific updates based on DTO fields.
                if (!string.IsNullOrWhiteSpace(updateDto.RoomNumber) && room.RoomNumber.ToLower() != updateDto.RoomNumber.ToLower())
                {
                    var existingRoomWithSameNumber = await _unitOfWork.Rooms.FindAsync(r => r.HotelAssetId == hotelAssetId && r.RoomNumber.ToLower() == updateDto.RoomNumber.ToLower());
                    if (existingRoomWithSameNumber.Any(r => r.Id != roomId))
                    {
                        throw new InvalidOperationException($"Room number '{updateDto.RoomNumber}' already exists in this hotel.");
                    }
                }
                //Handle Image Updates (Replacing all existing images if newImageFiles are provided).
                if (newImageFiles != null && newImageFiles.Any())
                {
                    //Get old ImageKit IDs for deletion from cloud storage (before clearing collection).
                    var oldImageCloudIds = room.RoomImages?.Select(ri => ri.ImageId).ToList();

                    if (room.RoomImages != null)
                    {
                        room.RoomImages.Clear(); 
                    }

                    //Delete files from cloud storage (Imagekit).
                    if (oldImageCloudIds != null && oldImageCloudIds.Any())
                    {
                        var deleteResult = await _fileStorageService.DeleteMultipleFilesAsync(oldImageCloudIds);
                        if (!deleteResult.OverallSuccess)
                        {
                            _logger.LogWarning($"Failed to delete some old images for room {roomId} from Imagekit. Details: {deleteResult.Message}");
                        }
                    }

                    // Upload new image files to Imagekit.
                    var uploadResult = await _fileStorageService.UploadMultipleFilesAsync(newImageFiles, $"hotels/{hotelAssetId}/rooms");
                    if (uploadResult.OverallSuccess)
                    {

                        room.RoomImages = uploadResult.UploadResults.Where(r => r.Success)
                            .Select((uploadedFile, index) => new RoomImage
                            {
                                ImageId = uploadedFile.Id,
                                ImageURL = uploadedFile.Url,
                                RoomId = room.Id


                            }).ToList();
                        _logger.LogInformation($"Successfully uploaded {room.RoomImages.Count} new images for room {room.RoomNumber}.");
                    }
                    else
                    {
                        _logger.LogError($"Some or all new images failed to upload for room '{room.RoomNumber}'. Details: {uploadResult.Message}");
                        room.RoomImages = new List<RoomImage>(); // Explicitly set to empty list if new upload fails
                    }
                }

                _mapper.Map(updateDto, room);

                // 6. Update the entity in the repository and save changes.
               
                _unitOfWork.Rooms.Update(room);
                await _unitOfWork.CompleteAsync(); // This triggers cascade delete and add of RoomImages.

                _logger.LogInformation($"Room '{room.RoomNumber}' (ID: {room.Id}) updated successfully for Hotel {hotelAssetId}.");

                // 7. Re-fetch the updated room with details for comprehensive response DTO.
                var updatedRoomWithDetails = await _unitOfWork.Rooms.GetRoomByIdWithDetailsAsync(roomId);
                if (updatedRoomWithDetails == null) throw new Exception("Updated room not found after save and detail fetch.");

                return _mapper.Map<RoomResponseDto>(updatedRoomWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating room ID: {roomId} for Hotel {hotelAssetId}.");
                throw;
            }

        }

 
    }
}
