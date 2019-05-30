﻿using System.Threading.Tasks;
using EventBus.Infrastructure.Abstractions;
using EventBus.Infrastructure.Models;
using EventBus.Sample.ChildService.Services;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.ChildService.IntegrationEventHandlers
{
    public class MainIntegrationEventHandler : IIntegrationEventHandler<MainServiceModel>
    {
        private readonly ILogger<MainIntegrationEventHandler> _logger;
        private readonly IChildService _childService;

        public MainIntegrationEventHandler(ILogger<MainIntegrationEventHandler> logger, IChildService childService)
        {
            _logger = logger;
            _childService = childService;
        }

        public async Task HandleAsync(MainServiceModel @event)
        {
            _logger.LogInformation($"Handling rabbit integration event: {@event.Name}, {@event.Content}");
            await _childService.DoSomethingAsync().ConfigureAwait(false);
        }
    }
}