using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Mappings
{
    public static class AuthorMapping
    {
        public static Author CreateToEntity(this CreateAuthorDto createDto)
        {
            return new Author
            {
                Name = createDto.Name,
                Birthdate = createDto.Birthdate
            };
        }

        public static void UpdateFromDto(this Author entity, UpdateAuthorDto updateDto)
        {
            entity.Name = updateDto.Name;
            entity.Birthdate = updateDto.Birthdate;
        }

        public static AuthorDto ToDto(this Author entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new AuthorDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Birthdate = entity.Birthdate
            };
        }
    }
}
