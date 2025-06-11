using BookStoreApi.Entities;
namespace BookStoreApi.Entities
{
    public class Publisher
    {
        public Publisher()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string Address { get; set; }

        //1 to many rel
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
