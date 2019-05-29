using System.Threading.Tasks;

namespace EventBus.Infrastructure.Abstractions
{
    public interface IIntegrationEventHandler<in T>
    {
        Task HandleAsync(T @event);
    }
}
