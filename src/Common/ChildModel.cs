using EventBus.Infrastructure;

namespace Common
{
    public class ChildModel : IIntegrationEvent
    {
        public string Value { get; set; }
    }
}
