using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty").WithMessage("{PropertyName} is required.")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid").WithMessage("{PropertyName} is not a valid email address.");
        }
    }
}
