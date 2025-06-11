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

        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public PublisherDto Publisher { get; set; } = null!;
    }
}
