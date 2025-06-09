namespace BookStoreApi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }=string.Empty;
        //public string Author { get; set; }
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }= string.Empty;

        //1 to many relationship  **
        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; } = null!;
        // **
        //Many to many relationship **
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        // 8*
}
}
