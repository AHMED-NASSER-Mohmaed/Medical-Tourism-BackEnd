using Elagy.Core.Enums;

namespace Elagy.Core.DTOs.Pagination
{
    // Represents the pagination request coming from the client
    public class PaginationParameters
    {
        private const int MaxPageSize = 50; // Maximum allowed page size to prevent abuse
        public int PageNumber { get; set; } = 1; // Default to first page

        private int _pageSize = 10; // Default page size
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public Status? UserStatus { get; set; } 
        public string? SearchTerm { get; set; }
        public string? hospitalId { get; set; }
        public int? SpecialtyId {  get; set; }
        public string? FilterDoctorId { get; set; }

        public int? FilterDayOfWeekId { get; set; }
        public DateOnly? FilterStartDate { get; set; }
        public DateOnly? FilterEndDate { get; set; }
        public bool? FilterIsActive { get; set; }// general  status filter except users
        public int? specialtyScheduleId {  get; set; }
        public string? HotelAssetId { get; set; } 
        public RoomCategory? RoomType { get; set; }  //for website filter
        public RoomStatus? RoomStatus { get; set; } //for admin flter
        public DriverStatus? DriverStatus {  get; set; }

        public CarType? CarType { get; set; }
        public CarStatus? CarStatus { get; set; }

        public decimal? MinPrice { get; set; }  
        public decimal? MaxPrice { get; set; } 
        public int? MinOccupancy { get; set; } 
        public int? MaxOccupancy { get; set; } 
        public int? FilterGovernorateId { get; set; } //filter service providers by location for website 
        public AppointmentStatus? AppointmentStatus { get; set; }







    }
}
