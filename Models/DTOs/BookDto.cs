namespace BookStoreApi.Models.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
       // public int AuthorId { get; set; }//Note: check this later I think I should remove it 
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }

        public AuthorDto Author { get; set; } = null!;
        public ICollection<PublisherDto> Publishers { get; set; } = new List<PublisherDto>();
    }
}
