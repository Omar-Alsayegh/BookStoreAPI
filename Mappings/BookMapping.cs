using BookStoreApi.Entities;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookStoreApi.Mappings
{
    public static class BookMapping
    {
        //Convert the createbook entity to book entity
        public static Book CreateToBook(this CreateBookDto createBookDto)
        {
            if (createBookDto == null)
            {
                throw new ArgumentNullException(nameof(createBookDto), "CreateBookDto cannot be null.");
            }
            return new Book
            {
                Title = createBookDto.Title,
                PublicationYear = createBookDto.PublicationYear,
                Price = createBookDto.Price,
                Content = createBookDto.Content,
                PublisherId = createBookDto.PublisherId,

            };
        }
        //convert the book entity to update and then be updated
        public static void UpdateToBook(this Book entity, UpdateBookDto updateBookDto)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Book entity cannot be null for update.");
            }
            if (updateBookDto == null)
            {
                throw new ArgumentNullException(nameof(updateBookDto), "UpdateBookDto cannot be null.");
            }
            entity.Title = updateBookDto.Title;
            entity.PublicationYear = updateBookDto.PublicationYear;
            entity.Price = updateBookDto.Price;
            entity.Content = updateBookDto.Content;
            entity.PublisherId = updateBookDto.PublisherId;
        }
        //convert the book entity to bookdto
        public static BookDto ToBookDto(this Book entity)
        {
            if (entity == null)
            {

                throw new ArgumentNullException(nameof(entity), "Book entity cannot be null.");
            }
            if (entity.Publisher == null)
            {
                throw new InvalidOperationException("Author navigation property was not loaded for Book ToDto conversion.");
            }
            if (entity.BookContentPhotos == null)
            {
                throw new InvalidOperationException("BookContentPhotos navigation property was not loaded for Book DTO conversion.");
            }
            return new BookDto
            {
                Id = entity.Id,
                Title = entity.Title,
                PublicationYear = entity.PublicationYear,
                Price = entity.Price,
                Content = entity.Content,
                Publisher = entity.Publisher.ToDto(),
                StockQuantity = entity.StockQuantity,
                CoverImageUrl = entity.CoverImageUrl,
                //ContentImageUrls = entity.BookContentPhotos.Select(bcp => bcp.ImageUrl).ToList(),
                ContentImageUrls = entity.BookContentPhotos?.Select(bcp =>
                    $"/api/books/content-photos/{bcp.Id}"
                ).ToList() ?? new List<string>(),
                Authors = entity.BookAuthors?.Select(ba => ba.Author.ToDto()).ToList(),
            };


        }
    }
}
