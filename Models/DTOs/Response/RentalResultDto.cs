using BookStoreApi.Entities;

namespace BookStoreApi.Models.DTOs.Response
{
    public class RentalResultDto
    {
        public RentalDto? Rental { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public RentalStatus Status { get; set; }
    }
}
