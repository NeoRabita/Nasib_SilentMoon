using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Domain.Errors
{
    public static class TopicErrors
    {
        public static readonly Error SomeNotFound = Error.NotFound(
            "Topics.SomeNotFound",
            "One or more selected topics do not exist or are inactive.");
    }
}
