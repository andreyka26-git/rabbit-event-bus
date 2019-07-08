using System.Threading.Tasks;
using EventBus.Infrastructure.Abstractions;

namespace EventBus.Infrastructure.Rabbitmq.EventSubscribers
{
    /// <summary>
    /// This is the wrapper, needed to resolve scoped services inside eventHandler
    /// </summary>
    public interface IEventMapper
    {
        /// <summary>
        /// Resolve eventHandler from scope, and handle event via it.
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="event">Event</param>
        /// <returns>Task</returns>
        Task MapEventAsync<T>(T @event)
            where T : class, new();
    }
}