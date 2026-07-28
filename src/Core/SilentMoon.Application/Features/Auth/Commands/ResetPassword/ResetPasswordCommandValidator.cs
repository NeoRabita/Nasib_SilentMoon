using FluentValidation;

namespace SilentMoon.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Auth.Email.NotEmpty")
                .EmailAddress().WithErrorCode("Auth.Email.Invalid");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithErrorCode("Auth.Otp.NotEmpty")
                .Length(6).WithErrorCode("Auth.Otp.Length").WithMessage("{PropertyName} must be 6 digits.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode("Auth.Password.NotEmpty")
                .MinimumLength(8).WithErrorCode("Auth.Password.MinLength").WithMessage("{PropertyName} must be at least 8 characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithErrorCode("Auth.Password.Mismatch").WithMessage("Passwords do not match.");
        }
    }
}