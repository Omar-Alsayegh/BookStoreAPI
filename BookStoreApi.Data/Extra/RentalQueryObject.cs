using BookStoreApi.Entities;

namespace BookStoreApi.Extra
{
    public class RentalQueryObject:IQueryObject
    {
        public string? BookTitle { get; set; }
        public string? CustomerEmail { get; set; }
        public RentalStatus? Status { get; set; } 
        public DateTimeOffset? RentalDateFrom { get; set; } 
        public DateTimeOffset? RentalDateTo { get; set; }  

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false; // Default to ascending

        // Pagination properties
        private const int MaxPageSize = 10; // Maximum number of items per page
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5; 
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; 
        }

        public DateTime? ExpectedReturnDateFrom { get; set; }
        public DateTime? ExpectedReturnDateTo { get; set; }
    }
}
