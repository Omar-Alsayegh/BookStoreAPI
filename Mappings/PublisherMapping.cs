using BookStoreApi.Entities;
using BookStoreApi.Extensions;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Mappings
{
    public static class PublisherMapping
    {
        public static Publisher ToEntity(this CreatePublisherDto createDto)
        {
            return new Publisher
            {
                Name = createDto.Name
            };
        }

        public static void UpdateFromDto(this Publisher entity, UpdatePublisherDto updateDto)
        {
            entity.Name = updateDto.Name;
        }

        public static PublisherDto ToDto(this Publisher entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new PublisherDto
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }
    }
}
