namespace EventBus.Infrastructure.Rabbitmq.Serializer
{
    public interface IMessageSerializer
    {
        byte[] SerializeMessage<T>(T message);
        T DeserializeMessage<T>(byte[] body);
    }
}