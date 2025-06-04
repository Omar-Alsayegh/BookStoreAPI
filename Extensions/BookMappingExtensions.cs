using BookStoreApi.Models;
using BookStoreApi.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookStoreApi.Extensions
{
    public static class BookMappingExtensions
    {
      //Convert the createbook entity to book entity
      public static Book CreateToBook (this CreateBookDto createBookDto)
        {
            if (createBookDto == null)
            {
                throw new ArgumentNullException(nameof(createBookDto), "CreateBookDto cannot be null.");
            }
            return new Book
            {
                Title = createBookDto.Title,
                Author=createBookDto.Author,
                PublicationYear = createBookDto.PublicationYear,
                Price=createBookDto.Price,
                Content= createBookDto.Content,

            };
        }
        //convert the book entity to update and then be updated
        public static void UpdateToBook(this Book entity, UpdateBookDto updateBookDto) {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Book entity cannot be null for update.");
            }
            if (updateBookDto == null)
            {
                throw new ArgumentNullException(nameof(updateBookDto), "UpdateBookDto cannot be null.");
            }
            entity.Title = updateBookDto.Title;
            entity.Author = updateBookDto.Author;
            entity.PublicationYear = updateBookDto.PublicationYear;
            entity.Price = updateBookDto.Price;
            entity.Content = updateBookDto.Content;
        }
        //convert the book entity to bookdto
        public static BookDto ToBookDto(this Book entity) {
            if (entity == null) { 
                
                throw new ArgumentNullException(nameof(entity), "Book entity cannot be null.");
            }
            return new BookDto
            {
               Title = entity.Title,
               Author = entity.Author,
               PublicationYear = entity.PublicationYear,
               Price=entity.Price,
               Content = entity.Content
            };
           

        }
    }
}
