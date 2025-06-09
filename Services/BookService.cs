using BookStoreApi.Entities;
using BookStoreApi.Extensions;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IPublisherRepository _publisherRepository;
        public BookService(IBookRepository bookRepository, IPublisherRepository publisherRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
        }
        public async Task<BookDto> CreateBookAsync(CreateBookDto createDto)
        {
            var publisherExists = await _publisherRepository.ExistsAsync(createDto.PublisherId);
            if (!publisherExists)
            {
                throw new InvalidOperationException($"Publisher with ID {createDto.PublisherId} does not exist.");
            }

            var authors = new List<Author>();
            foreach (var authorId in createDto.AuthorIds)
            {
                var author = await _authorRepository.GetByIdAsync(authorId);
                if (author == null)
                {
                    throw new InvalidOperationException($"Author with ID {authorId} does not exist.");
                }
                authors.Add(author);
            }
            if (!authors.Any())
            {
                throw new InvalidOperationException("Cannot create a book without authors.");
            }
            var newbook = createDto.CreateToBook();
            newbook.BookAuthors = authors.Select(a => new BookAuthor { Author = a, Book = newbook }).ToList();
            await _bookRepository.AddAsync(newbook);
            await _bookRepository.SaveChangesAsync();
            var createdBookWithRelations = await _bookRepository.GetByIdAsync(newbook.Id);
            if (createdBookWithRelations == null)
            {
                throw new InvalidOperationException("Newly created book could not be retrieved from the database.");
            }

            return createdBookWithRelations.ToBookDto();

        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var DelBook = _bookRepository.GetByIdAsync(id);
            if (DelBook != null)
            {
                await _bookRepository.DeleteAsync(id);
                await _bookRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(b => b.ToBookDto()).ToList();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book?.ToBookDto();
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto updateDto)
        {
            var updatedbook = await _bookRepository.GetByIdAsync(id);
            if (updatedbook != null)
            {
                return false;
            }

            var publisherExists = await _publisherRepository.ExistsAsync(updateDto.PublisherId);
            if (!publisherExists)
            {
                throw new InvalidOperationException($"Publisher with ID {updateDto.PublisherId} does not exist.");
            }

            var newAuthors = new List<Author>();
            foreach (var authorId in updateDto.AuthorIds)
            {
                var author = await _authorRepository.GetByIdAsync(authorId);
                if (author == null)
                {
                    throw new InvalidOperationException($"Author with ID {authorId} does not exist.");
                }
                newAuthors.Add(author);
            }
            if (!newAuthors.Any())
            {
                throw new InvalidOperationException("A book must have at least one author.");
            }

            updatedbook.UpdateToBook(updateDto);
            updatedbook.PublisherId = updateDto.PublisherId;

            var currentAuthorIds = updatedbook.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
            var newAuthorIds = updateDto.AuthorIds.ToHashSet();

            foreach (var currentAuthorId in currentAuthorIds)
            {
                if (!newAuthorIds.Contains(currentAuthorId))
                {
                    var bookAuthorToRemove = updatedbook.BookAuthors.FirstOrDefault(ba => ba.AuthorId == currentAuthorId);
                    if (bookAuthorToRemove != null)
                    {
                        updatedbook.BookAuthors.Remove(bookAuthorToRemove);
                    }
                }
            }

            foreach (var newAuthorId in newAuthorIds)
            {
                if (!currentAuthorIds.Contains(newAuthorId))
                {
                    // Find the actual Author entity
                    var authorToAdd = newAuthors.FirstOrDefault(a => a.Id == newAuthorId);
                    if (authorToAdd != null)
                    {
                        updatedbook.BookAuthors.Add(new BookAuthor { Book = updatedbook, Author = authorToAdd });
                    }
                }
            }

            await _bookRepository.UpdateAsync(updatedbook);
            await _bookRepository.SaveChangesAsync();
            return true;

        }
    }
}
