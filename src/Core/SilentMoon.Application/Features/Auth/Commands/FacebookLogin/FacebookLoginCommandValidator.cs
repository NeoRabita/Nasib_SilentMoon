using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Commands.FacebookLogin
{
    public class FacebookLoginCommandValidator : AbstractValidator<FacebookLoginCommand>
    {
        public FacebookLoginCommandValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithErrorCode("Auth.AccessToken.NotEmpty").WithMessage("{PropertyName} is required.");
        }
    }
}
