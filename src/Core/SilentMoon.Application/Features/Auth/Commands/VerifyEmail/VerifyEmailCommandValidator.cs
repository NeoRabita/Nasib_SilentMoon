using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithErrorCode("Auth.Otp.NotEmpty").WithMessage("{PropertyName} is required.")
                .Length(6).WithErrorCode("Auth.Otp.Length").WithMessage("{PropertyName} must be 6 digits.");
        }
    }
}
