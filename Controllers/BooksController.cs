

using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Services;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var Books = _bookService.GetAllBooksAsync();
            return Ok(Books);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book= _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto createBook)
        {
            var newBook= await _bookService.CreateBookAsync(createBook);
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }
        [HttpPut]
        public async Task<ActionResult<BookDto>> PutBook(int id,UpdateBookDto updateBook)
        {
            var success = await _bookService.UpdateBookAsync(id, updateBook);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookService.DeleteBookAsync(id);

            if (!success)
            {
                return NotFound(); 
            }

            return NoContent(); 
        }
    }
}
