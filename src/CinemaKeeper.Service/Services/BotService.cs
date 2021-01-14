using System;
using System.Reflection;
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
        private readonly DiscordBotConfiguration _botConfiguration;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;

        public BotService(IServiceProvider services)
        {
            _services = services;

            _client = services.GetRequiredService<DiscordSocketClient>();
            _commandService = services.GetRequiredService<CommandService>();

            _botConfiguration = services.GetRequiredService<DiscordBotConfiguration>();

            _client.MessageReceived += HandleCommand;
            _commandService.CommandExecuted += OnCommandExecuted;

            _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
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

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context,
            IResult result)
        {
            if (!string.IsNullOrEmpty(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);

            var commandName = command.IsSpecified ? command.Value.Name : "unknown command";

            Log.Information($"Executed \"{commandName}\" for {context.User.Username}");
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
    }
}
