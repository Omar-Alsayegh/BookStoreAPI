using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize (Roles ="Admin,Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAllAuthors([FromQuery] AuthorQueryObject query)
        {
            var authors =await _authorService.GetAllAuthorsAsync(query);
            var authorsDto = authors.Select(a => a.ToDto()).ToList();
            return Ok(authorsDto);
        }
        [Authorize (Roles ="Admin,Employee")]
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
        [Authorize (Roles ="Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor([FromBody] CreateAuthorDto createAuthor)
        {
            var author = new Author
            {
                Name = createAuthor.Name,
                Birthdate = createAuthor.Birthdate
            };
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            author.CreatedBy = userId;          
            author.CreatedAt = DateTime.UtcNow;

            author.ModifiedAt = DateTime.UtcNow;
            author.ModifiedBy = userId;

            var createdEntity= await _authorService.CreateAuthorAsync(author);
            await _authorService.SaveChangesAsync();
            var createdAuthorDto = createdEntity.ToDto();
            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
        }

        [Authorize (Roles ="Admin,Employee")]
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

            return Ok(existingAuthors);

        }
        [Authorize (Roles ="Admin,Employee")]
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
