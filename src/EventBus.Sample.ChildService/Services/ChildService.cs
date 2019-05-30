using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EventBus.Sample.ChildService.Services
{
    public class ChildService : IChildService
    {
        private readonly ILogger<ChildService> _logger;

        public ChildService(ILogger<ChildService> logger)
        {
            _logger = logger;
        }

        public Task DoSomethingAsync()
        {
            _logger.LogInformation("Do somethings.");

            return Task.CompletedTask;
        }
    }
}
