using System;

namespace EventBus.Infrastructure
{
    public interface IEventBus : IDisposable
    {
        void Subscribe<TH, TE>(string exchangeName, string subscriberName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent;

        void Publish(IIntegrationEvent @event, string exchangeName);
    }
}