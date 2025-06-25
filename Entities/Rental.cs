using BookStoreApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApi.Entities
{
    public enum RentalStatus
    {
        Pending,
        Accepted,
        Rejected,
        Returned,
        Overdue,
        Cancelled
    }

    public class Rental: BaseEntity
    {
        public int Id { get; set; }


        public int BookId { get; set; }
        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }


        public string UserId { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public DateTime RentalDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public String? ApprovedBy { get; set; }
        public string? ReasonOfRejection { get; set; } 

        public RentalStatus Status { get; set; }

    }
}
