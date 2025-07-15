using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;

namespace BookStoreApi.Mappings
{
    public static class WishlistMapping
    {
        public static BookDto WishlistToBookDto(this Book book)
        {
            if (book == null) return null;

            // Ensure navigation properties are loaded before mapping
            // In your controller/service, you'll need .Include() for BookAuthors, Author, Publisher, BookContentPhotos
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear, 
                Price = book.Price,
                Content = book.Content,
                StockQuantity = book.StockQuantity,
                CoverImageUrl = book.CoverImageUrl,
                // Map Publisher (null check in case it's not loaded)
                Publisher = book.Publisher != null ? new PublisherDto { Id = book.Publisher.Id, Name = book.Publisher.Name } : null,
                // Map Authors (null check for BookAuthors collection)
                Authors = book.BookAuthors?.Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    Name = ba.Author.Name,
                    Birthdate = ba.Author.Birthdate
                }).ToList() ?? new List<AuthorDto>(), // Handle null collection gracefully
                // Map BookContentPhotos
                //BookContentPhotos = book.BookContentPhotos?.Select(photo => new BookContentPhotoDto
                //{
                //    Id = photo.Id,
                //    Url = photo.Url
                //}).ToList() ?? new List<BookContentPhotoDto>()
            };
        }

        // Mapper for Wishlist entity to WishlistItemDto
        public static WishlistItemDto ToWishlistItemDto(this Wishlist wishlistItem)
        {
            if (wishlistItem == null) return null;

            // Ensure Book navigation property is loaded before calling ToBookDto()
            return new WishlistItemDto
            {
                Id = wishlistItem.Id,
                AddedAt = wishlistItem.CreatedAt,
                // This calls the ToBookDto extension method for the nested book details
                Book = wishlistItem.Book?.WishlistToBookDto() // Null check for book in case it wasn't loaded
            };
        }
    }
}
