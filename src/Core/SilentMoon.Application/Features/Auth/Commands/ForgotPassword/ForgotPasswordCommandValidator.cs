using FluentValidation;

namespace SilentMoon.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid");
        }
    }
}