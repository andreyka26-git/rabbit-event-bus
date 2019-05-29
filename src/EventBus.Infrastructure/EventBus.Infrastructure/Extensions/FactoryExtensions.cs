using RabbitMQ.Client;

namespace EventBus.Infrastructure.Extensions
{
    public static class FactoryExtensions
    {
        public static bool IsAsyncConsumerAllowed(this IConnectionFactory factory)
        {
            var factoryImplementation = factory as ConnectionFactory;

            return factoryImplementation?.DispatchConsumersAsync != false;
        }
    }
}
