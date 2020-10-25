using CinemaKeeper.Service.Configurations;

using Discord.WebSocket;

namespace CinemaKeeper.Service.Adapters
{
    internal class DiscordBotConfigurationAdapter : IAdapter<DiscordSocketConfig>
    {
        private readonly DiscordBotConfiguration _configuration;

        private DiscordBotConfigurationAdapter(DiscordBotConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IAdapter<DiscordSocketConfig> Create(DiscordBotConfiguration configuration) =>
            new DiscordBotConfigurationAdapter(configuration);

        public DiscordSocketConfig Convert()
        {
            var discordSocketConfig = new DiscordSocketConfig
            {
                LogLevel = _configuration.LogLevel
            };

            return discordSocketConfig;
        }
    }
}