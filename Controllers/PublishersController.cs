using BookStoreApi.Models.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAllPublishers()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return Ok(publishers);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisherById(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return Ok(publisher);
        }
        [HttpPost]
        public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] CreatePublisherDto createDto)
        {

            var newPublisher = await _publisherService.CreatePublisherAsync(createDto);
            return CreatedAtAction(nameof(GetPublisherById), new { id = newPublisher.Id }, newPublisher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto updateDto)
        {

            var isUpdated = await _publisherService.UpdatePublisherAsync(id, updateDto);
            if (!isUpdated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var isDeleted = await _publisherService.DeletePublisherAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
