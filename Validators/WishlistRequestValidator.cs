using BookStoreApi.Migrations;
using BookStoreApi.Models.DTOs;
using BookStoreApi.Models.DTOs.Request;
using BookStoreApi.Models.DTOs.Response;
using FluentValidation;

namespace BookStoreApi.Validators
{
    public class WishlistRequestValidator : AbstractValidator<AddBookToWishlistDto>
    {
        public WishlistRequestValidator()
        {
            RuleFor(W => W.BookId)
                .NotEmpty().WithMessage("The Book Id can not be empty. ")
                .GreaterThan(1).WithMessage("The Book Id can not be less than 1. ");

        }
    }
}
