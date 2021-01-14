using System;

using CinemaKeeper.Service.Adapters;
using CinemaKeeper.Service.Configurations;

using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaKeeper.Service.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services,
            IConfiguration configuration)
        {
            var discordBotConfiguration = new DiscordBotConfiguration();
            configuration.Bind("DiscordBot", discordBotConfiguration);
            services.AddSingleton(discordBotConfiguration);

            return services;
        }

        public static IServiceCollection AddDiscordClient(this IServiceCollection services,
            DiscordBotConfiguration configuration)
        {
            var configAdapter = DiscordBotConfigurationAdapter.Create(configuration) as IAdapter<DiscordSocketConfig>;
            var discordSocketConfig = configAdapter.Convert();

            var client = new DiscordSocketClient(discordSocketConfig);

            return services.AddSingleton(client);
        }

        public static IServiceCollection AddCommandService(this IServiceCollection services,
            DiscordBotConfiguration configuration)
        {
            var configAdapter = DiscordBotConfigurationAdapter.Create(configuration) as IAdapter<CommandServiceConfig>;
            var commandServiceConfig = configAdapter.Convert();

            var service = new CommandService(commandServiceConfig);

            return services.AddSingleton(service);
        }
    }
}
