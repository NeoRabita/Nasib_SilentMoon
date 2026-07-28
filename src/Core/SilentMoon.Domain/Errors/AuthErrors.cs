using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Domain.Errors
{
    public static class AuthErrors
    {
        public static readonly Error EmailNotUnique = Error.Conflict(
           "Auth.EmailNotUnique",
           "The provided email is already registered.");

        public static readonly Error InvalidCredentials = Error.Unauthorized(
            "Auth.InvalidCredentials",
            "Email or password is incorrect.");

        public static readonly Error EmailNotVerified = Error.Forbidden(
            "Auth.EmailNotVerified",
            "Email is not verified. Please verify your email first.");

        public static readonly Error EmailAlreadyVerified = Error.Conflict(
            "Auth.EmailAlreadyVerified",
            "Email is already verified.");

        public static readonly Error InvalidOtp = Error.Validation(
            "Auth.InvalidOtp",
            "OTP code is invalid or expired.");

        public static readonly Error InvalidRefreshToken = Error.Unauthorized(
            "Auth.InvalidRefreshToken",
            "Refresh token is invalid, expired or revoked.");

        public static readonly Error UserNotFound = Error.NotFound(
            "Auth.UserNotFound",
            "The user with the specified email was not found.");

        public static readonly Error UserInactive = Error.Forbidden(
            "Auth.UserInactive",
            "This account has been deactivated.");

        public static readonly Error InvalidExternalToken = Error.Unauthorized(
            "Auth.InvalidExternalToken",
            "External provider token is invalid.");

        public static readonly Error WrongProvider = Error.Conflict(
            "Auth.WrongProvider",
            "This email is registered with a different authentication provider.");

        public static readonly Error PasswordResetNotAllowedForExternalProvider = Error.Conflict(
            "Auth.PasswordResetNotAllowedForExternalProvider",
            "This account uses external login (Google/Facebook) and has no password to reset.");
    }
}
