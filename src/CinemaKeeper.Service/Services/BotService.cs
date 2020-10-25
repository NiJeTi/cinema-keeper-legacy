using System.Threading;
using System.Threading.Tasks;

using CinemaKeeper.Service.Adapters;
using CinemaKeeper.Service.Configurations;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Hosting;

namespace CinemaKeeper.Service.Services
{
    internal class BotService : BackgroundService
    {
        private readonly DiscordBotConfiguration _botConfiguration;
        private readonly DiscordSocketClient _client;

        public BotService(DiscordBotConfiguration botConfiguration)
        {
            _botConfiguration = botConfiguration;

            var discordSocketConfig =
                DiscordBotConfigurationAdapter
                   .Create(botConfiguration)
                   .Convert();

            _client = new DiscordSocketClient(discordSocketConfig);
            _client.Log += DiscordClientLogger.LogMessage;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.LoginAsync(TokenType.Bot, _botConfiguration.Token);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _client.StartAsync();

            while (!cancellationToken.IsCancellationRequested)
                await Task.Yield();

            await _client.StopAsync();
        }
    }
}