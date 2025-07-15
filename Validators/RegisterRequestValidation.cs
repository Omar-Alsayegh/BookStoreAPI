using BookStoreApi.Models.DTOs.Auth;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class RegisterRequestValidation : AbstractValidator<RegisterDto>
    {
        public RegisterRequestValidation()
        {
            RuleFor(r => r.EmailAddress)
                .NotEmpty().WithMessage("Email cannot be Empty!")
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password cannot be empty!")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty().WithMessage("The Confirm Password cannot be empty.")
                .Equal(r => r.Password).WithMessage("The password and confirmation password do not match.");

            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("The Firstname cannot be empty.");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("The Lastname cannot be empty.");
            
        }
    }
}
