using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Auth.Events
{
    public enum OtpPurpose
    {
        Register = 1,
        Resend = 2
    }

    public class OtpEmailEvent
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string OtpCode { get; set; }
        public OtpPurpose Purpose { get; set; }
    }
}
