using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.Infrastructure.Rabbitmq.Queues
{
    public interface IQueue<T> : IDisposable
        where T : class, new()
    {
        bool IsConnected { get; }
        void Connect();
        void Publish(T message);
        void SubscribeWithAsync(Func<T, IModel, BasicDeliverEventArgs, Task> callback, bool autoAck = false);
        void Subscribe(Action<T, IModel, BasicDeliverEventArgs> callback, bool autoAck = false);
    }
}
