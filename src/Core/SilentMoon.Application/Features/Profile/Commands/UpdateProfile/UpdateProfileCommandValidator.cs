using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithErrorCode("Profile.FirstName.Empty").WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithErrorCode("Profile.FirstName.MaxLength")
                .When(x => x.FirstName is not null);

            RuleFor(x => x.LastName)
                .NotEmpty().WithErrorCode("Profile.LastName.Empty").WithMessage("{PropertyName} cannot be empty.")
                .MaximumLength(50).WithErrorCode("Profile.LastName.MaxLength")
                .When(x => x.LastName is not null);

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500).WithErrorCode("Profile.AvatarUrl.MaxLength")
                .Must(url => System.Uri.TryCreate(url, System.UriKind.Absolute, out _))
                    .WithErrorCode("Profile.AvatarUrl.Invalid").WithMessage("{PropertyName} must be a valid URL.")
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
        }
    }
}
