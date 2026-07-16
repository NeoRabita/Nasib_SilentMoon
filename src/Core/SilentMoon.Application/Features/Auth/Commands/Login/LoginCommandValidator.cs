using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : FluentValidation.AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty").WithMessage("{PropertyName} is required.")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid");

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode("Auth.Password.NotEmpty").WithMessage("{PropertyName} is required.");
        }


    }
}
