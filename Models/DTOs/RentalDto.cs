using BookStoreApi.Entities;

namespace BookStoreApi.Models.DTOs
{
    public class RentalDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!; 
        public string CustomerEmail { get; set; } = null!; 
        public DateTime RentalDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ReasonOfRejection { get; set; }
        public RentalStatus Status { get; set; }
        
    }
}
