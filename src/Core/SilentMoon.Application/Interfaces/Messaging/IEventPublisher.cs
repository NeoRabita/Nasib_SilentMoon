using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Messaging
{
    public interface IEventPublisher
    {
        
        Task PublishAsync<T>(string routingKey, T message) where T : class;
    }
}
