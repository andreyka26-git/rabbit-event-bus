using System;
using System.Threading.Tasks;
using EventBus.Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Infrastructure.Rabbitmq.EventSubscribers
{
    public class EventMapper : IEventMapper
    {
        private readonly IServiceProvider _serviceProvider;

        public EventMapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task MapEventAsync<T>(T @event)
            where T : class, new()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var integrationEventHandler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
                await integrationEventHandler.HandleAsync(@event).ConfigureAwait(false);
            }
        }
    }
}
