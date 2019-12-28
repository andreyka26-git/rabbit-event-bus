using EventBus.Infrastructure;

namespace Common
{
    public class MainServiceModel : IIntegrationEvent
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
