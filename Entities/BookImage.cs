using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApi.Entities
{
    public class BookImage:BaseEntity
    {
        //public string ImageUrl { get; set; } = string.Empty;

        // Foreign Key to Book
        public int BookId { get; set; }
        public byte[] ImageData { get; set; } = null!;

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; } = null!;

    }
}
