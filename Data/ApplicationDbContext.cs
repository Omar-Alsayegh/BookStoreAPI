using Microsoft.EntityFrameworkCore;
using BookStoreApi.Entities;

namespace BookStoreApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author>authors { get; set; }
        public DbSet<Publisher> publisher { get; set; }
        public DbSet<BookPublisher> bookPublishers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().Property(b => b.Price).HasPrecision(18, 2);

            //One to many relationship 
            modelBuilder.Entity<Book>()
                .HasOne(b=>b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookPublisher>()
                .HasKey(bp => new { bp.PublisherId, bp.BookId });

            modelBuilder.Entity<BookPublisher>()
                .HasOne(bp => bp.Book)
                .WithMany(b => b.BookPublishers)
                .HasForeignKey(bp => bp.BookId);
            modelBuilder.Entity<BookPublisher>()
                .HasOne(bp => bp.Publisher)
                .WithMany(b => b.BookPublishers)
                .HasForeignKey(bp => bp.PublisherId);
        }

    }
}
