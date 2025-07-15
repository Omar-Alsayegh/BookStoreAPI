using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class PublisherCreateRequestValidator : AbstractValidator<CreatePublisherDto>
    {
        public PublisherCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name Cannot be Empty! ")
                .MaximumLength(20).WithMessage("Name Cannot Exceed 20 Characters. ");
        }
    }
}
