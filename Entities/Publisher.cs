namespace BookStoreApi.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<BookPublisher> BookPublishers { get; set; } = new List<BookPublisher>();
    }
}
