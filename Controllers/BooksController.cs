

using BookStoreApi.Entities;
using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Repositories;
using BookStoreApi.Services;
using BookStoreApi.Services.FileStorage;
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
        private readonly IFileStorageService _fileStorageService;
        private readonly IGenericRepository<BookImage> _bookImageRepository;
        public BooksController(IBookService bookService, IAuthorService authorService, ILogger<BooksController> logger, IFileStorageService fileStorageService, IGenericRepository<BookImage> bookImageRepository)
        {
            _bookService = bookService;
            _authorService = authorService;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _bookImageRepository = bookImageRepository;
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
        [Authorize(Roles = "Admin,Employee")]
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
        [Authorize(Roles = "Admin,Employee")]
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
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the book." });
            }

        }
        [Authorize(Roles = "Admin,Employee")]
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

        [HttpPut("{id}/cover-photo")]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UploadBookCover(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG files are allowed.");
            }

            // Delete old cover photo if it exists
            if (!string.IsNullOrEmpty(book.CoverImageUrl))
            {
                _fileStorageService.DeleteFile(book.CoverImageUrl);
            }

            // Save new cover photo
            string newCoverUrl = await _fileStorageService.SaveFileAsync(file, "Books"); // "Books" is the subfolder
            book.CoverImageUrl = newCoverUrl;

            await _bookService.UpdateBookAsync(book); 
            await _bookService.SaveChangesAsync();

            // Return the updated book DTO with the new URL
            var bookDto = book.ToBookDto(); 
            return Ok(bookDto);
        }

        [HttpDelete("{id}/cover-photo")]
        public async Task<IActionResult> DeleteBookCover(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            if (string.IsNullOrEmpty(book.CoverImageUrl))
            {
                return NotFound("Book does not have a cover photo.");
            }

            _fileStorageService.DeleteFile(book.CoverImageUrl);
            book.CoverImageUrl = null; // Clear the URL in the database

            await _bookService.UpdateBookAsync(book);
            await _bookService.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpPost("{bookId}/content-photos")]
        [Consumes("multipart/form-data")] // Important for file uploads
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadBookContentPhoto(int bookId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            // 1. Get the book from the database
            // Ensure GetBookByIdAsync loads BookContentPhotos (which we updated it to do)
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return NotFound($"Book with ID {bookId} not found.");
            }

            try
            {     
                //2.Save the file
                // Define a folder path specific to book content photos (e.g., "bookcontent")
                //DATABASE STORAGE
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }



                //LOCAL STORAGE

                //var folderName = "bookcontent";
                //var imageUrl = await _fileStorageService.SaveFileAsync(file, folderName);

                //if (string.IsNullOrEmpty(imageUrl))
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the image file.");
                //}

                // 3. Create a new BookContentPhoto entity
                var bookContentPhoto = new BookImage
                {
                   // ImageUrl = imageUrl,
                    BookId = book.Id, 
                    ImageData=imageData,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = User.Identity?.Name ?? "System"
                };

                // 4. Add the BookContentPhoto to the book's collection and to the database context
                book.BookContentPhotos.Add(bookContentPhoto);
                await _bookService.UpdateBookAsync(book); // This should call the repository's update and save change
                await _bookService.SaveChangesAsync();
                // 5. Return the updated BookDto
                var updatedBook = await _bookService.GetBookByIdAsync(bookId);
                return Ok(updatedBook.ToBookDto());
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error uploading book content photo for Book ID {BookId}", bookId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while uploading the photo: {ex.Message}");
            }
        }

        [HttpGet("content-photos/{photoId}")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookContentPhoto(int photoId)
        {
            try
            {
                var bookImage = await _bookImageRepository.GetByIdAsync(photoId);

                if (bookImage == null || bookImage.ImageData == null || bookImage.ImageData.Length == 0)
                {
                    return NotFound("Book content photo not found or no image data available.");
                }

                // Determine the content type (MIME type) of the image.
                // This is crucial for the browser to display the image correctly.
                string contentType = "image/jpg"; 

                return File(bookImage.ImageData, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book content photo with ID {PhotoId}.", photoId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
