using SilentMoon.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Common.Extensions
{
    public static class UserServiceExtensions
    {
   
        public static int? GetUserIdAsInt(this IUserService userService)
        {
            var idStr = userService.GetUserId();
            return int.TryParse(idStr, out var id) ? (int?)id : null;
        }
    }
}

