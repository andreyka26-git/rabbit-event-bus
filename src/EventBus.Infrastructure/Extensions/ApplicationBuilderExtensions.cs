using EventBus.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void SubscribeOnIntegrationEvent<T, TH>(this IApplicationBuilder builder)
        where T: class, new()
        where TH : IIntegrationEventHandler<T>
        {
            var eventBus = builder.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.SubscribeWithAsync<T,TH>();
        }
    }
}
