using BookStoreApi.Entities;

namespace BookStoreApi.Models.DTOs
{
    public class BookDto
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }=string.Empty;
        public int StockQuantity { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public ICollection<string> ContentImageUrls { get; set; } = new List<string>();
        public PublisherDto Publisher { get; set; } = null!;
    }
}
