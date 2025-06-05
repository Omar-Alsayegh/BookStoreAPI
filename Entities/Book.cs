namespace BookStoreApi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public string Author { get; set; }
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }

        public int AuthorId { get; set; }
        public Author Author{ get; set; } = null!;
        public ICollection<BookPublisher> BookPublishers { get; set; } = new List<BookPublisher>();
}
}
