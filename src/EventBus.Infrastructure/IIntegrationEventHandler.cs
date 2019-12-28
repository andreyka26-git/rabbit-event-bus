using System.Threading.Tasks;

namespace EventBus.Infrastructure
{
    public interface IIntegrationEventHandler<TE>
        where TE: IIntegrationEvent
    {
        Task Handle(TE @event);
    }
}
