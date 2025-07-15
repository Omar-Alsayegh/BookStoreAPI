using BookStoreApi.Models.DTOs.Auth;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Validators
{
    public class ResetPasswordRequestValidator:AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress();

            RuleFor(r => r.Token)
                .NotEmpty().WithMessage("Token cannot be empty.");

            RuleFor(r => r.NewPassword)
                .NotEmpty().WithMessage("The new password cannot be empty.")
                .MinimumLength(8).WithMessage("The {0} must be at least {2} and at max {1} characters long.");

            RuleFor(r => r.ConfirmNewPassword)
                .NotEmpty().WithMessage("The password confirmation cannot be empty")
                .Equal(r => r.NewPassword).WithMessage("The new password and confirmation password do not match.");
        }

  
    }
}
