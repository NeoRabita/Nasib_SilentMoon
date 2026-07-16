using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Onboarding.Commands.CreateReminder;
using SilentMoon.Application.Features.Onboarding.Commands.DeleteReminder;
using SilentMoon.Application.Features.Onboarding.Commands.SetMyTopics;
using SilentMoon.Application.Features.Onboarding.Commands.UpdateReminder;
using SilentMoon.Application.Features.Onboarding.Queries.GetMyReminders;
using SilentMoon.Application.Features.Onboarding.Queries.GetMyTopics;
using SilentMoon.Application.Features.Onboarding.Queries.GetTopics;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Route("api/v{version:apiVersion}")]
    public class OnboardingController : BaseController
    {
       
        [HttpGet("topics")]
        public async Task<IResult> GetTopics()
        {
            var result = await Dispatcher.Send(new GetTopicsQuery());
            return HandleResult(result);
        }

   
        [Authorize]
        [HttpGet("me/topics")]
        public async Task<IResult> GetMyTopics()
        {
            var result = await Dispatcher.Send(new GetMyTopicsQuery());
            return HandleResult(result);
        }

 
        [Authorize]
        [HttpPut("me/topics")]
        public async Task<IResult> SetMyTopics([FromBody] SetMyTopicsCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

     
        [Authorize]
        [HttpGet("me/reminders")]
        public async Task<IResult> GetMyReminders()
        {
            var result = await Dispatcher.Send(new GetMyRemindersQuery());
            return HandleResult(result);
        }

 
        [Authorize]
        [HttpPost("me/reminders")]
        public async Task<IResult> CreateReminder([FromBody] CreateReminderCommand command)
        {
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

    
        [Authorize]
        [HttpPatch("me/reminders/{id:int}")]
        public async Task<IResult> UpdateReminder(int id, [FromBody] UpdateReminderCommand command)
        {
            command.Id = id;
            var result = await Dispatcher.Send(command);
            return HandleResult(result);
        }

 
        [Authorize]
        [HttpDelete("me/reminders/{id:int}")]
        public async Task<IResult> DeleteReminder(int id)
        {
            var result = await Dispatcher.Send(new DeleteReminderCommand(id));
            return HandleResult(result);
        }
    }
}
