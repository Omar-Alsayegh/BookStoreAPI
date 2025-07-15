using BookStoreApi.Models.DTOs.Auth;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class ForgetPasswordValidation : AbstractValidator<ForgotPasswordRequestDto>
    {
        public ForgetPasswordValidation()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email cannot be empty. ")
                .EmailAddress();
        }
    }
}
