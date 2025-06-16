using BookStoreApi.Extra;
using BookStoreApi.Mappings;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAllPublishers([FromQuery] PublisherQueryObject query)
        {
            var publishers = await _publisherService.GetAllPublishersAsync(query);
            var publishersDto = publishers.Select(p => p.ToDto()).ToList();
            return Ok(publishersDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisherById(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            var publisherDto = publisher.ToDto();
            return Ok(publisher);
        }
        [HttpPost]
        public async Task<ActionResult<PublisherDto>> CreatePublisher([FromBody] CreatePublisherDto createDto)
        {
            var newPublisherEntity = createDto.ToEntity();
            await _publisherService.CreatePublisherAsync(newPublisherEntity);
            await _publisherService.SaveChangesAsync();
            var createdPublisherDto = newPublisherEntity.ToDto();
            return CreatedAtAction(nameof(GetPublisherById), new { id = newPublisherEntity.Id }, newPublisherEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto updateDto)
        {
            var existingPublisherEntity = await _publisherService.GetPublisherByIdAsync(id);
            if (existingPublisherEntity == null)
            {
                return NotFound();
            }
            existingPublisherEntity.UpdateFromDto(updateDto);
            await _publisherService.UpdatePublisherAsync(existingPublisherEntity);
            await _publisherService.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher([FromRoute] int id)
        {
            var publisherToDeleteEntity = await _publisherService.GetPublisherByIdAsync(id);
            if (publisherToDeleteEntity == null)
            {
                return NotFound();
            }
            await _publisherService.DeletePublisherAsync(publisherToDeleteEntity);
            await _publisherService.SaveChangesAsync();
            return NoContent();
        }

    }
}
