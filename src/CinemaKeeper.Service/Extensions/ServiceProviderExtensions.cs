using CinemaKeeper.Service.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaKeeper.Service.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static IServiceCollection AddDiscordBotConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var discordBotConfiguration = new DiscordBotConfiguration();
            configuration.Bind("DiscordBot", discordBotConfiguration);

            return services.AddSingleton(discordBotConfiguration);
        }
    }
}