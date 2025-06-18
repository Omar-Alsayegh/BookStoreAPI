

using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookService bookService, IAuthorService authorService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _authorService = authorService;
            _logger = logger;
        }
     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] QueryObject query)
        {
            var Books = await _bookService.GetAllBooksAsync(query);
            var booksDto = Books.Select(b => b.ToBookDto()).ToList();
            return Ok(Books);
        }
       // [Authorize (Roles ="Admin,Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookdto = book.ToBookDto();
            return Ok(book);
        }
        [Authorize (Roles ="Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook([FromBody] CreateBookDto createBook)
        {
            try
            {
                var newBook = createBook.CreateToBook();

                if (createBook.AuthorIds != null && createBook.AuthorIds.Any())
                {
                    newBook.BookAuthors = new List<BookAuthor>();
                    foreach (var authorId in createBook.AuthorIds)
                    {
                        var author = await _authorService.GetAuthorByIdAsync(authorId);
                        if (author == null)
                        {
                            return BadRequest($"Author with ID {authorId} not found.");
                        }
                        newBook.BookAuthors.Add(new BookAuthor { AuthorId = authorId, Book = newBook });
                    }
                }
                else
                {
                    return BadRequest("Book must have at least one author.");
                }

                var createdBook = await _bookService.CreateBookAsync(newBook);
                await _bookService.SaveChangesAsync();
                var loadedBook = await _bookService.GetBookWithAuthorsAsync(createBook.Title);
                var createdBookDto = loadedBook.ToBookDto();
                return CreatedAtAction(nameof(GetBook), new { id = createdBookDto.Id }, createdBookDto);

            }
            catch (Exception e)
            {
                _logger.LogError("An Error occured", e.Message);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while processing your request.",
                    exceptionMessage = e.Message
                });
            }


            }
        [Authorize(Roles ="Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> PutBook(int id, UpdateBookDto updateBook)
        {
            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }
            existingBook.UpdateToBook(updateBook);
            existingBook.PublisherId = updateBook.PublisherId;
            if (updateBook.AuthorIds == null || !updateBook.AuthorIds.Any())
            {
                return BadRequest("A book must have at least one author."); 
            }
            var currentAuthorIds = existingBook.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
            var newAuthorIds = updateBook.AuthorIds.ToHashSet();
            var bookAuthorsToRemove = existingBook.BookAuthors
                .Where(ba => !newAuthorIds.Contains(ba.AuthorId))
                .ToList();
            foreach (var bookAuthor in bookAuthorsToRemove)
            {
                existingBook.BookAuthors.Remove(bookAuthor); 
            }
            foreach (var newAuthorId in newAuthorIds)
            {
                if (!currentAuthorIds.Contains(newAuthorId))
                {
                    var author = await _authorService.GetAuthorByIdAsync(newAuthorId);
                    if (author == null)
                    {
                        return BadRequest($"Author with ID {newAuthorId} not found."); 
                    }

                    existingBook.BookAuthors.Add(new BookAuthor { BookId = existingBook.Id, AuthorId = newAuthorId });
                }
            }
            try
            {
                await _bookService.UpdateBookAsync(existingBook);

                await _bookService.SaveChangesAsync();

                return Ok(existingBook);
            }
            catch (InvalidOperationException ) 
            {
                return BadRequest();
            }
            catch (Exception ) 
            {
               
                return StatusCode(500, new { message = "An error occurred while updating the book."});
            }
            
        }
        [Authorize (Roles ="Admin,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var BooktoDelete = await _bookService.GetBookByIdAsync(id);
            if (BooktoDelete == null)
            {
                return NotFound();
            }
            await _bookService.DeleteBookAsync(BooktoDelete);
            await _bookService.SaveChangesAsync();

            return NoContent();
        }
    }
}
