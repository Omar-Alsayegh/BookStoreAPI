

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
            var Books = await _bookService.GetAllBooksAsync();
            return Ok(Books);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook(CreateBookDto createBook)
        {
            try
            {
                var newBook = await _bookService.CreateBookAsync(createBook);
                return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the book.", details = ex.Message });
            }


            }
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> PutBook(int id, UpdateBookDto updateBook)
        {
            if (id == 0) 
            {
                return BadRequest("Invalid ID provided in route.");
            }
            try 
            {
            var success = await _bookService.UpdateBookAsync(id, updateBook);

            if (!success)
            {
                return NotFound();
            }

            return NoContent(); 
            }catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the book.", details = ex.Message });
            }
            
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
