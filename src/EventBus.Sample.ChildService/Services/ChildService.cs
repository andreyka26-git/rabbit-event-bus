using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.ChildService.Services
{
    public class ChildService : IChildService
    {
        private readonly ILogger<ChildService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ChildService(ILogger<ChildService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task DoSomethingAsync()
        {
            _logger.LogInformation("Do somethings.");

            try
            {
                _serviceProvider.CreateScope();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Probably service provider already disposed: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
