namespace BookStoreApi.Entities
{
    public class BookPublisher
    {
        public int BookId { get; set; }
        public int PublisherId { get; set; }
        public Book Book { get; set; } = null!;
        public Publisher Publisher { get; set; } = null!;
    }
}
