using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class BookRequestValidator: AbstractValidator<UpdateBookDto>
    {
        public BookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.") // Maps to [Required]
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters."); // Maps to [StringLength(200)]

            // Rule for PublicationYear
            RuleFor(x => x.PublicationYear)
                .InclusiveBetween(1000, DateTime.Now.Year + 5) // You had 2100, I'll use current year + 5 as a more dynamic max.
                .WithMessage($"Publication Year must be between 1000 and {DateTime.Now.Year + 5}."); // Maps to [Range(1000, 2100)]

            // Rule for Price
            RuleFor(x => x.Price)
                .InclusiveBetween(0.01m, 10000.00m).WithMessage("Price must be between 0.01 and 10000.00."); // Maps to [Range(0.01, 10000.00)]

            // Rule for Content (if it's not null, validate length)
            RuleFor(x => x.Content)
                .MaximumLength(10000000).WithMessage("Content cannot exceed 10,000,000 characters.") // Maps to [StringLength(10000000)]
                .When(x => x.Content != null); // Only validate if Content is provided/not null

            // Rule for PublisherId
            RuleFor(x => x.PublisherId)
                .NotEmpty().WithMessage("Publisher ID is required.") // Maps to [Required] (for int, NotEmpty is good for 0, NotNull for nullable)
                .GreaterThan(0).WithMessage("Publisher ID must be a positive integer."); // Assuming ID should be positive

            // Rule for AuthorIds (List<int>)
            RuleFor(x => x.AuthorIds)
                .NotEmpty().WithMessage("A book must have at least one author.") // Maps to [MinLength(1)]
                .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("All Author IDs must be positive integers.")
                .When(x => x.AuthorIds != null); // Ensure the list itself is not null if it's optional
            // Note: If the list can truly be empty but not null, just use .MinimumLength(1) or .Must(list => list.Count >=1)
            // But .NotEmpty() is generally suitable for collections that must contain items.
        }
    }
}
