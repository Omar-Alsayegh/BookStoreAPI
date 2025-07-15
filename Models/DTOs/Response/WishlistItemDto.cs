namespace BookStoreApi.Models.DTOs.Response
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public BookDto Book { get; set; } 
        public DateTimeOffset AddedAt { get; set; }
    }
}
