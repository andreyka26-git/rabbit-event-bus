namespace EventBus.Infrastructure.Abstractions
{
    public interface IEventBus
    {
        /// <summary>
        /// Publishes event to event bus
        /// </summary>
        /// <typeparam name="T">Type of message (event)</typeparam>
        /// <param name="message">Message (event)</param>
        void Publish<T>(T message) where T : class, new();


        /// <summary>
        /// Subscribe to Event with appropriate event handler 
        /// </summary>
        /// <typeparam name="T">Message (event) type</typeparam>
        /// <typeparam name="TH">Handler, which will listen messages</typeparam>
        void SubscribeWithAsync<T, TH>() 
            where T : class, new()
            where TH : IIntegrationEventHandler<T>;
    }
}
