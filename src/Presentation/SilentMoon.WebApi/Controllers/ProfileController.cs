using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Profile.Commands.UpdateProfile;
using SilentMoon.Application.Features.Profile.Queries.GetMe;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/me")]
    public class ProfileController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetMe()
        {
            var result = await Dispatcher.Send(new GetMeQuery());
            return HandleResult(result);
        }

        [HttpPatch]
        public async Task<IResult> UpdateMe([FromBody] UpdateProfileCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }
    }
}
