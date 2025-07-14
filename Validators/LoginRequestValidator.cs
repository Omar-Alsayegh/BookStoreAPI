using BookStoreApi.Models.DTOs.Auth;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("The email cannot be empty")
                .EmailAddress();

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("The password cannot be empty.");
       
        }
    }
}
