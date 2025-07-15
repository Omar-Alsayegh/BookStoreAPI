using Microsoft.EntityFrameworkCore;
using BookStoreApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace BookStoreApi.Data.Data
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
        public DbSet<Wishlist> Wishlist { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().Property(b => b.Price).HasPrecision(18, 2);

            modelBuilder.Entity<BookAuthor>()
               .HasKey(ba => new { ba.BookId, ba.AuthorId }); 

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book) 
                .WithMany(b => b.BookAuthors) 
                .HasForeignKey(ba => ba.BookId).IsRequired().OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);

            // Configure Book-Publisher One-to-Many relationship (Publisher has many Books)
            //modelBuilder.Entity<Publisher>()
            //    .HasMany(p => p.Books)
            //    .WithOne(b => b.Publisher) 
            //    .HasForeignKey(b => b.PublisherId) 
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(p => p.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Book>()
            //    .HasMany(p => p.Rentals)
            //    .WithOne(b => b.Book)
            //    .HasForeignKey(b => b.BookId)
            //    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rental>()
                .HasOne(p => p.Book)
                .WithMany(p => p.Rentals)
                .HasForeignKey(p => p.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(p => p.Rentals)
            //    .WithOne(b => b.User)
            //    .HasForeignKey(b => b.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rental>()
                .HasOne(p => p.User)
                .WithMany(p => p.Rentals)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookImage>()
                .HasOne(bcp => bcp.Book)         
                .WithMany(b => b.BookContentPhotos) 
                .HasForeignKey(bcp => bcp.BookId)   
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(b => b.ApplicationUser)
                .WithMany(b => b.WishlistItems)
                .HasForeignKey(b => b.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Wishlist>()
                .HasOne(b=> b.Book)
                .WithMany()
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Wishlist>()
                .HasIndex(wl => new { wl.ApplicationUserId, wl.BookId })
                .IsUnique();
        }
    }
}
