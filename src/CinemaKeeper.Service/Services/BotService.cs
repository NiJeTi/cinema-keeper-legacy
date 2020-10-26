using System;
using System.Threading;
using System.Threading.Tasks;

using CinemaKeeper.Service.Configurations;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace CinemaKeeper.Service.Services
{
    internal class BotService : BackgroundService
    {
        private readonly IServiceProvider _services;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        private readonly DiscordBotConfiguration _botConfiguration;

        public BotService(IServiceProvider services)
        {
            _services = services;

            _client = services.GetService<DiscordSocketClient>();
            _commandService = services.GetService<CommandService>();

            _botConfiguration = services.GetService<DiscordBotConfiguration>();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += HandleCommand;
            _commandService.CommandExecuted += OnCommandExecuted;

            await _client.LoginAsync(TokenType.Bot, _botConfiguration.Token);
            await _client.StartAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
        }

        public override void Dispose()
        {
            _client.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
                await Task.Yield();
        }

        private async Task HandleCommand(SocketMessage message)
        {
            if (!(message is SocketUserMessage command))
                return;

            var cmdStartPos = 0;

            if (!command.HasStringPrefix(_botConfiguration.Prefix, ref cmdStartPos)
                || command.HasMentionPrefix(_client.CurrentUser, ref cmdStartPos)
                || command.Author.IsBot)
                return;

            var commandContext = new SocketCommandContext(_client, command);

            await _commandService.ExecuteAsync(commandContext, cmdStartPos, _services);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);

            string commandName = command.IsSpecified ? command.Value.Name : "unknown command";

            Log.Information($"Executed \"{commandName}\" for {context.User.Username}");
        }
    }
}