using Microsoft.Extensions.Configuration;

namespace EventBus.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetRabbitValue<T>(this IConfiguration config, string configKey)
        {
            return config.GetSection(Consts.Consts.DefaultRabbitConfigurationSection).GetValue<T>(configKey);
        }
    }
}

