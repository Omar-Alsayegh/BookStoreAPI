using Microsoft.EntityFrameworkCore;
using BookStoreApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace BookStoreApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Rental> Rentals { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().Property(b => b.Price).HasPrecision(18, 2);

            modelBuilder.Entity<BookAuthor>()
               .HasKey(ba => new { ba.BookId, ba.AuthorId }); // Define composite primary key

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book) // BookAuthor has one Book
                .WithMany(b => b.BookAuthors) // Book has many BookAuthors
                .HasForeignKey(ba => ba.BookId).IsRequired().OnDelete(DeleteBehavior.Restrict); // Foreign key to Book

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author) // BookAuthor has one Author
                .WithMany(a => a.BookAuthors) // Author has many BookAuthors
                .HasForeignKey(ba => ba.AuthorId); // Foreign key to Author

            // Configure Book-Publisher One-to-Many relationship (Publisher has many Books)
            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Books) // Publisher has many Books
                .WithOne(b => b.Publisher) // Book has one Publisher
                .HasForeignKey(b => b.PublisherId) // Foreign key in Book
                .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent deleting publisher if books exist
                                                    // Use .OnDelete(DeleteBehavior.Cascade) for automatic deletion of books.
                                                    // Let's stick with Restrict for now, it's safer.

            modelBuilder.Entity<Book>()
                .HasMany(p => p.Rentals)
                .WithOne(b => b.Book)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(p => p.Rentals)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
