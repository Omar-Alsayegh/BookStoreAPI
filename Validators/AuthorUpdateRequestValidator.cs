using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class AuthorUpdateRequestValidator : AbstractValidator<UpdateAuthorDto>
    {
        public AuthorUpdateRequestValidator()
        {
            RuleFor(a => a.Name)
             .NotEmpty().WithMessage("The name cannot be emoty.")
             .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");

            RuleFor(a => a.Birthdate)
                .InclusiveBetween(new DateTime(1800 - 01 - 01), DateTime.UtcNow)
                .WithMessage($"The Birthdate must be between 1,January,1800 and {DateTime.UtcNow}");
        }
    }
}
