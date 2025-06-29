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

        public int? SpecialtyId {  get; set; }
        public int? FilterDayOfWeekId { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public string? FilterDoctorId { get; set; }
        public bool? FilterIsRecurring { get; set; }
        public bool? FilterIsActive { get; set; }
        

        // order acs , desc 

    }
}
