using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;   
using SilentMoon.Application.DTOs.Account;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IFacebookAuthService
    {
        Task<ExternalUserInfo> ValidateAccessTokenAsync(string accessToken);
    }
}
