using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithErrorCode("Auth.FirstName.NotEmpty").WithMessage("{PropertyName} is required.")
                .MaximumLength(50).WithErrorCode("Auth.FirstName.MaxLength").WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithErrorCode("Auth.LastName.NotEmpty").WithMessage("{PropertyName} is required.")
                .MaximumLength(50).WithErrorCode("Auth.LastName.MaxLength").WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty").WithMessage("{PropertyName} is required.")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid").WithMessage("{PropertyName} is not a valid email address.")
                .MaximumLength(256).WithErrorCode("Auth.Email.MaxLength");

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode("Auth.Password.NotEmpty").WithMessage("{PropertyName} is required.")
                .MinimumLength(8).WithErrorCode("Auth.Password.MinLength").WithMessage("{PropertyName} must be at least 8 characters.")
                .Matches("[A-Z]").WithErrorCode("Auth.Password.Uppercase").WithMessage("{PropertyName} must contain at least one uppercase letter.")
                .Matches("[0-9]").WithErrorCode("Auth.Password.Digit").WithMessage("{PropertyName} must contain at least one digit.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithErrorCode("Auth.ConfirmPassword.NotMatch").WithMessage("Passwords do not match.");
        }
    }
}
