using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Onboarding.Commands.SetMyTopics
{
    public class SetMyTopicsCommandValidator : AbstractValidator<SetMyTopicsCommand>
    {
        public SetMyTopicsCommandValidator()
        {
            RuleFor(x => x.TopicIds)
                .NotNull().WithErrorCode("Topics.Ids.Null").WithMessage("TopicIds is required (can be empty).");

            RuleForEach(x => x.TopicIds)
                .GreaterThan(0).WithErrorCode("Topics.Ids.Invalid").WithMessage("Topic id must be greater than 0.");

            RuleFor(x => x.TopicIds)
                .Must(ids => ids == null || ids.Count <= 50)
                .WithErrorCode("Topics.Ids.TooMany").WithMessage("You can select at most 50 topics.");
        }
    }
}
