using BookStoreApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs
{
    public class RentalStatusUpdateRequestDto
    {
        public String? ReasonOfRejection { get; set; }

    }
}
