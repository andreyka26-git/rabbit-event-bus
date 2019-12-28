using System.Threading.Tasks;
using Common;
using EventBus.Infrastructure;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.MainService.IntegrationEventHandlers
{
    public class ChildIntegrationEventHandler : IIntegrationEventHandler<ChildModel>
    {
        private readonly ILogger<ChildIntegrationEventHandler> _logger;

        public ChildIntegrationEventHandler(ILogger<ChildIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ChildModel @event)
        {
            _logger.LogInformation($"Handling rabbit integration event: {@event.Value}");
        }
    }
}