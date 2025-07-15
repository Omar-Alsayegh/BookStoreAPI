using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class PublisherUpdateRequestValidator : AbstractValidator<UpdatePublisherDto>
    {
        public PublisherUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is Required.")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");
        }
    }
}
