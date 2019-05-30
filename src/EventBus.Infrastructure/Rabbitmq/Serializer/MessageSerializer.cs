using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventBus.Infrastructure.Rabbitmq.Serializer
{
    public class MessageSerializer : IMessageSerializer
    {
        public byte[] SerializeMessage<T>(T message)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    var serializer = CreateDefaultSerializer();

                    serializer.Serialize(writer, message);
                }

                return stream.ToArray();
            }
        }

        public T DeserializeMessage<T>(byte[] body)
        {
            using (var stream = new MemoryStream(body))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = CreateDefaultSerializer();

                var value = serializer.Deserialize<T>(jsonReader);
                return value;
            }
        }

        private JsonSerializer CreateDefaultSerializer()
        {
            var serializer = JsonSerializer.CreateDefault();

            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Formatting = Formatting.None;

            return serializer;
        }
    }
}
