using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Mappings
{
    public static class PublisherMapping
    {
        public static Publisher ToEntity(this CreatePublisherDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));
            return new Publisher
            {
                Name = createDto.Name,
                Address = createDto.Address
            };
        }

        public static void UpdateFromDto(this Publisher entity, UpdatePublisherDto updateDto)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));

            entity.Name = updateDto.Name;
            entity.Address = updateDto.Address;
        }

        public static PublisherDto ToDto(this Publisher entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new PublisherDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address
            };
        }
    }
}
