using System;
using System.IO;
using System.Threading.Tasks;
using EventBus.Infrastructure.Extensions;
using EventBus.Infrastructure.Rabbitmq.Serializer;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.Infrastructure.Rabbitmq.Queues
{
    public class FanoutQueue<T> : IQueue<T>
        where T : class, new()
    {
        public bool IsConnected => _connection != null;

        private readonly IConnectionFactory _factory;
        private readonly IMessageSerializer _serializer;
        private readonly ILogger<FanoutQueue<T>> _logger;
        private readonly string _exchangeName;
        private readonly object _isConnectedLockObject = new object();

        private IConnection _connection;
        private IModel _model;
        private string _queueName;
        private string _consumeTag;

        public FanoutQueue(IConnectionFactory factory,
            IMessageSerializer serializer,
            string exchangeName,
            ILogger<FanoutQueue<T>> logger = null)
        {
            _factory = factory;
            _serializer = serializer;
            _exchangeName = exchangeName;
            _logger = logger;
        }

        public void Connect()
        {
            if (IsConnected)
                return;

            lock (_isConnectedLockObject)
            {
                if (IsConnected)
                    return;

                _connection = _factory.CreateConnection();
                _model = _connection.CreateModel();

                _model.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, true);
            }
        }

        public void Publish (T message)
        {
            var encodedMessage = _serializer.SerializeMessage(message);
            _model.BasicPublish(_exchangeName, "", null, encodedMessage);
        }

        public void Subscribe(Action<T, IModel, BasicDeliverEventArgs> callback, bool autoAck = false)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += (sender, args) =>
            {
                try
                {
                    var message = _serializer.DeserializeMessage<T>(args.Body);
                    callback(message, _model, args);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Error occured when processing received callback from rabbitmq. {ex.Message}");
                }
                if (!autoAck)
                    _model.BasicAck(args.DeliveryTag, false);
            };

            _queueName = _model.QueueDeclare().QueueName;
            _model.QueueBind(_queueName, _exchangeName, "");

            _consumeTag = _model.BasicConsume(_queueName, autoAck, consumer);
        }

        public void SubscribeWithAsync(Func<T, IModel, BasicDeliverEventArgs, Task> callback, bool autoAck = false)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (!_factory.IsAsyncConsumerAllowed())
                throw new InvalidDataException("You don't specify async consumer property.");

            var consumer = new AsyncEventingBasicConsumer(_model);

            consumer.Received += async (sender, args) =>
            {
                try
                {
                    var message = _serializer.DeserializeMessage<T>(args.Body);
                    await callback(message, _model, args).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Error occured when processing received callback from rabbitmq. {ex.Message}");
                }

                if (!autoAck)
                    _model.BasicAck(args.DeliveryTag, false);
            };

            _queueName = _model.QueueDeclare().QueueName;
            _model.QueueBind(_queueName, _exchangeName, "");

            _consumeTag = _model.BasicConsume(_queueName, autoAck, consumer);
        }

        // Default implementation of Dispose pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //there is no need to dispose factory because aspnet core will do it for us.
            _connection.Dispose();
            _model.Dispose();
        }
    }
}
