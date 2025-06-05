using BookStoreApi.Models.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<AuthorDto>>> GetAllAuthors()
        {
            var authors =await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor([FromBody] CreateAuthorDto createAuthor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newAuthor = await _authorService.CreateAuthorAsync(createAuthor);
            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isUpdated = await _authorService.UpdateAuthorAsync(id, updateAuthor);
            if (!isUpdated)
            {
                return NotFound();
            }
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var isDelete = await _authorService.DeleteAuthorAsync(id);
            if (!isDelete)
            {
                return NotFound();
            }
            return NoContent();

        }
    }
}
