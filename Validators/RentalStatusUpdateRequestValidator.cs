using BookStoreApi.Entities;
using BookStoreApi.Models.DTOs;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class RentalStatusUpdateRequestValidator : AbstractValidator<RentalStatusUpdateRequestDto>
    {
        public RentalStatusUpdateRequestValidator()
        {
            RuleFor(R => R.ReasonOfRejection)
                .MaximumLength(100).WithMessage("The Reason of Rejection cannot be more than 100 characters.");

            RuleFor(R => R.rentalStatus)
                .IsInEnum().WithMessage("Must be an Enum")
                .NotNull().WithMessage("Cannot be Null/Empty");
        }
    }
}
