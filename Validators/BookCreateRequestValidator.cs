using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class BookCreateRequestValidator:AbstractValidator<CreateBookDto>
    {
        public BookCreateRequestValidator() 
        {
            RuleFor(x => x.Title)
               .NotEmpty().WithMessage("Title is required.")
               .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.PublicationYear)
                .InclusiveBetween(1000, DateTime.Now.Year + 5)
                .WithMessage($"Publication Year must be between 1000 and {DateTime.Now.Year + 5}.");

            RuleFor(x => x.Price)
                .InclusiveBetween(0.01m, 10000.00m).WithMessage("Price must be between 0.01 and 10000.00.");

            RuleFor(x => x.Content)
                .MaximumLength(10000000).WithMessage("Content cannot exceed 10,000,000 characters.")
                .When(x => x.Content != null);

            RuleFor(x => x.PublisherId)
                .NotEmpty().WithMessage("Publisher ID is required.")
                .GreaterThan(0).WithMessage("Publisher ID must be a positive integer.");

            RuleFor(x => x.AuthorIds)
                .NotEmpty().WithMessage("A book must have at least one author.")
                .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("All Author IDs must be positive integers.")
                .When(x => x.AuthorIds != null);
        }
    }
}
