using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Entities;
using BookStoreApi.Repositories;
using BookStoreApi.Extra;
using BookStoreApi.Models.DTOs.Response;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        //public async Task<ActionResult<IAsyncEnumerable<AuthorDto>>> GetAllAuthors()
        //{
        //    var authors = await _authorService.GetAllAuthorsAsync();
        //    authors = (IEnumerable<Author>)authors.Select(static a => a.ToDto());
        //    return Ok(authors);
        //}
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAllAuthors([FromQuery] AuthorQueryObject query)
        {
            var authors =await _authorService.GetAllAuthorsAsync(query);
            var authorsDto = authors.Select(a => a.ToDto()).ToList();
            return Ok(authorsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            
            if (author == null)
            {
                return NotFound();
            }
            var authordto =author.ToDto();
            return Ok(authordto);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor([FromBody] CreateAuthorDto createAuthor)
        {
            var newAuthor =createAuthor.CreateToEntity();
            var createdEntity= await _authorService.CreateAuthorAsync(newAuthor);
            await _authorService.SaveChangesAsync();
            var createdAuthorDto = createdEntity.ToDto();
            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, newAuthor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthor)
        {

            var existingAuthors = await _authorService.GetAuthorByIdAsync(id);
            if (existingAuthors == null)
            {
                return NotFound();
            }
            existingAuthors.UpdateFromDto(updateAuthor);
            await _authorService.UpdateAuthorAsync(existingAuthors);
            await _authorService.SaveChangesAsync();

            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor([FromRoute] int id)
        {
            var AuthortoDelete = await _authorService.GetAuthorByIdAsync(id);
            if (AuthortoDelete == null)
            {
                return NotFound();
            }
            await _authorService.DeleteAuthorAsync(AuthortoDelete);
            await _authorService.SaveChangesAsync();
           
            return NoContent(); 
        }
    }
}
