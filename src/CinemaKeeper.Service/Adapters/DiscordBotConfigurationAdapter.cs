using System;
using System.Linq;

using CinemaKeeper.Service.Configurations;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace CinemaKeeper.Service.Adapters
{
    internal class DiscordBotConfigurationAdapter : IAdapter<DiscordSocketConfig>, IAdapter<CommandServiceConfig>
    {
        private readonly DiscordBotConfiguration _discordBotConfiguration;

        private DiscordBotConfigurationAdapter(DiscordBotConfiguration discordBotConfiguration)
        {
            _discordBotConfiguration = discordBotConfiguration;
        }

        public static DiscordBotConfigurationAdapter Create(DiscordBotConfiguration configuration) =>
            new(configuration);

        CommandServiceConfig IAdapter<CommandServiceConfig>.Convert()
        {
            var commandServiceConfig = new CommandServiceConfig
            {
                LogLevel = Enum.GetValues(typeof(LogSeverity)).Cast<LogSeverity>().Max()
            };

            return commandServiceConfig;
        }

        DiscordSocketConfig IAdapter<DiscordSocketConfig>.Convert()
        {
            var discordSocketConfig = new DiscordSocketConfig
            {
                LogLevel = Enum.GetValues(typeof(LogSeverity)).Cast<LogSeverity>().Max()
            };

            return discordSocketConfig;
        }
    }
}