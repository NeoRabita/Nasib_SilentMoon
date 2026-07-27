using SilentMoon.Application.Features.Auth.Events;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace SilentMoon.Application.Common.Services
{
    public class OtpDispatcher : IOtpDispatcher
    {
        private readonly IOtpService _otpService;
        private readonly IEventPublisher _publisher;

        public OtpDispatcher(IOtpService otpService, IEventPublisher publisher)
        {
            _otpService = otpService;
            _publisher = publisher;
        }

        public async Task SendAsync(string email, string firstName, OtpPurpose purpose)
        {
            var otp = await _otpService.GenerateAsync(email);

            await _publisher.PublishAsync("otp.send", new OtpEmailEvent
            {
                Email = email,
                FirstName = firstName,
                OtpCode = otp,
                Purpose = purpose
            });
        }
    }
}