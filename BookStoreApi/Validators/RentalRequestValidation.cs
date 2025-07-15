using BookStoreApi.Models.DTOs;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class RentalRequestValidation: AbstractValidator<RentalRequestDto>
    {
        public RentalRequestValidation() 
        {
            RuleFor(r => r.BookId)
                .NotEmpty().WithMessage("Book Id is needed. ")
                .GreaterThanOrEqualTo(1).WithMessage("Id cannot be less than 1.");

            RuleFor(r => r.DesiredDurationDays)
                .ExclusiveBetween(1, 365).WithMessage("Rentak duration must be between 1 and 365 days.")
                .GreaterThan(1).WithMessage("Number cannot be negative");
        }
    }
}
