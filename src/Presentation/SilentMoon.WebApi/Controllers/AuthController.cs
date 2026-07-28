using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Auth.Commands.FacebookLogin;
using SilentMoon.Application.Features.Auth.Commands.GoogleLogin;
using SilentMoon.Application.Features.Auth.Commands.Login;
using SilentMoon.Application.Features.Auth.Commands.Logout;
using SilentMoon.Application.Features.Auth.Commands.RefreshToken;
using SilentMoon.Application.Features.Auth.Commands.Register;
using SilentMoon.Application.Features.Auth.Commands.ResendOtp;
using SilentMoon.Application.Features.Auth.Commands.ForgotPassword;
using SilentMoon.Application.Features.Auth.Commands.ResetPassword;
using SilentMoon.Application.Features.Auth.Commands.VerifyEmail;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        [HttpPost("register")]
        public async Task<IResult> Register([FromBody] RegisterCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<IResult> Login([FromBody] LoginCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("verify-email")]
        public async Task<IResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }
        [HttpPost("forgot-password")]
        public async Task<IResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IResult> Logout([FromBody] LogoutCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("oauth/google")]
        public async Task<IResult> GoogleLogin([FromBody] GoogleLoginCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

        [HttpPost("oauth/facebook")]
        public async Task<IResult> FacebookLogin([FromBody] FacebookLoginCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }
    }
}