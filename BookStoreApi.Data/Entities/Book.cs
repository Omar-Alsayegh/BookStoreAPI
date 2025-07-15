namespace BookStoreApi.Entities
{
    public class Book: BaseEntity
    {
        public Book() //always do all these lists like this 
        {
            BookAuthors = new HashSet<BookAuthor>();
            Rentals = new HashSet<Rental>();
            BookContentPhotos = new HashSet<BookImage>();
        }

        public int Id { get; set; }
        public string Title { get; set; }=string.Empty;
        //public string Author { get; set; }
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }= string.Empty;
        public string? CoverImageUrl { get; set; }
        public int StockQuantity { get; set; }

        //1 to many relationship  **
        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; } = null!;
        // **
        //Many to many relationship **
        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<Rental>? Rentals { get; set; }

        public ICollection<BookImage> BookContentPhotos { get; set; }
        // 8*
    }
}
