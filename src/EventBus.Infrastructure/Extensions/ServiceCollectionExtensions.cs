using System;
using System.Linq;
using EventBus.Infrastructure.Abstractions;
using EventBus.Infrastructure.Rabbitmq.EventSubscribers;
using EventBus.Infrastructure.Rabbitmq.Queues;
using EventBus.Infrastructure.Rabbitmq.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all necessary services for publishing to event bus.
        /// You need to specify section "RabbitOptions" in your appsettings and specify "Host"(string) - which is appropriate rabbitmq host,
        /// "AsyncConsumer"(bool) - to specify if receive will have accept async or sync callback and "ExchangeName" to know how to name your exchange.
        /// Important: calling this method you can only publishing events, not subscribe. For subscribing call Overload this method with 2 Type parameters.
        /// </summary>
        /// <typeparam name="T">Message (event) type</typeparam>
        /// <param name="services">Services</param>
        /// <param name="config">Configuration</param>
        /// <param name="exchangeName">Exchange name on rabbit</param>
        /// <param name="loggerFactory">Logger Factory</param>
        public static void AddIntegrationEventServices<T>(this IServiceCollection services, IConfiguration config,
            ILoggerFactory loggerFactory, string exchangeName)
            where T : class, new()
        {
            AddBasicIntegrationEventServices<T>(services, config, loggerFactory, exchangeName);
        }

        /// <summary>
        /// Adds necessary services for publishing and subscribing to event bus.
        /// You need to specify section "RabbitOptions" in your appsettings and specify "Host"(string) - which is appropriate rabbitmq host,
        /// "AsyncConsumer"(bool) - to specify if receive will have accept async or sync callback and "ExchangeName" to know how to name your exchange.
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <typeparam name="TH">Type of event handler</typeparam>
        /// <param name="services">Services</param>
        /// <param name="config">Configuration</param>
        /// <param name="loggerFactory">Logger Factory</param>
        /// <param name="exchangeName">Name of exchange on rabbit</param>
        public static void AddIntegrationEventServices<T, TH>(this IServiceCollection services, IConfiguration config,
            ILoggerFactory loggerFactory, string exchangeName)
            where T : class, new()
            where TH : class, IIntegrationEventHandler<T>
        {
            AddBasicIntegrationEventServices<T>(services, config, loggerFactory, exchangeName);
            
            if (!services.IsTypeInjected<IIntegrationEventHandler<T>>())
                services.AddTransient<IIntegrationEventHandler<T>, TH>();
        }

        /// <summary>
        /// Add connection factory into IOC container. Need to be a single method and single connection
        /// because it may be used by each connection.
        /// </summary>
        public static void AddConnectionFactory(this IServiceCollection services, IConfiguration config)
        {
            if (!services.IsTypeInjected<IConnectionFactory>())
            {
                services.AddSingleton<IConnectionFactory>(sp =>
                {
                    var connectionFactory = new ConnectionFactory();

                    var isAsyncConsumer = config.GetRabbitValue<bool>(Consts.Consts.DefaultRabbitConsumerTypeName);

                    //need to use async consumers
                    if (isAsyncConsumer)
                        connectionFactory.DispatchConsumersAsync = true;

                    var rabbitUri =
                        new Uri(config.GetRabbitValue<string>(Consts.Consts.DefaultRabbitHostConfigurationName));

                    connectionFactory.Uri = rabbitUri;

                    return connectionFactory;
                });
            }
        }

        /// <summary>
        /// Injects queue with fanout behavior.
        /// If that method will not find IConnectionFactory or IMessageSerializer - it will inject defaults.
        /// </summary>
        /// <typeparam name="T">Type of model you want to subscribe or publish via queue</typeparam>
        /// <param name="services">Services</param>
        /// <param name="config">Configuration</param>
        /// <param name="loggerFactory">Logger factory to create logger</param>
        /// <param name="exchangeName">Exchange name</param>
        public static void AddFanoutQueue<T>(this IServiceCollection services,
            IConfiguration config, 
            ILoggerFactory loggerFactory, 
            string exchangeName)
            where T : class, new()
        {
            if (!services.IsTypeInjected<IMessageSerializer>())
                services.AddSingleton<IMessageSerializer, MessageSerializer>();

            if (!services.IsTypeInjected<IConnectionFactory>())
                services.AddConnectionFactory(config);

            services.AddSingleton<IQueue<T>>(provider => new FanoutQueue<T>(
                provider.GetService<IConnectionFactory>(),
                provider.GetService<IMessageSerializer>(),
                exchangeName,
                loggerFactory.CreateLogger<FanoutQueue<T>>()));
        }

        private static void AddBasicIntegrationEventServices<T>(IServiceCollection services, IConfiguration config,
            ILoggerFactory loggerFactory, string exchangeName)
            where T : class, new()
        {
            if (!services.IsTypeInjected<IEventBus>())
                services.AddSingleton<IEventBus, Rabbitmq.EventBus>();

            if (!services.IsTypeInjected<IQueue<T>>())
                services.AddFanoutQueue<T>(config, loggerFactory, exchangeName);

            if (!services.IsTypeInjected<IEventMapper>())
                services.AddSingleton<IEventMapper, EventMapper>();
        }

        /// <summary>
        /// Checks either Type T has already registered in IOC
        /// </summary>
        /// <typeparam name="T">Type to be checked</typeparam>
        /// <param name="services">Services</param>
        /// <returns>Value, indicates, where type is registered</returns>
        private static bool IsTypeInjected<T>(this IServiceCollection services)
        {
            return services.OfType<T>().Any();
        }
    }
}
