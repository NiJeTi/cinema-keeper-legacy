using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CinemaKeeper.Service.Configurations;
using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Resources;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

using Serilog;

namespace CinemaKeeper.Service.Services
{
    internal class BotService : BackgroundService
    {
        private readonly DiscordBotConfiguration _botConfiguration;
        private readonly IStringLocalizer<Localization> _localizer;

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;

        public BotService(IServiceProvider services)
        {
            _services = services;

            _client = services.GetRequiredService<DiscordSocketClient>();
            _commandService = services.GetRequiredService<CommandService>();

            _botConfiguration = services.GetRequiredService<DiscordBotConfiguration>();
            _localizer = services.GetRequiredService<IStringLocalizer<Localization>>();

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
            (_commandService as IDisposable).Dispose();

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                var response = GetErrorResponse(result);

                if (!string.IsNullOrEmpty(response))
                    await context.Channel.SendMessageAsync(response);
            }

            var commandName = command.IsSpecified ? command.Value.Name : "UNKNOWN COMMAND";
            Log.Information($"Executed \"{commandName}\" for {context.User}");
        }

        private async Task HandleCommand(SocketMessage message)
        {
            if (message is not SocketUserMessage command)
                return;

            var argPos = 0;

            var isInvalidCommand = !command.HasStringPrefix(_botConfiguration.Prefix, ref argPos)
                || command.HasMentionPrefix(_client.CurrentUser, ref argPos)
                || command.Author.IsBot
                || command.Content.Length == 1;

            if (isInvalidCommand)
                return;

            var commandContext = new SocketCommandContext(_client, command);
            await _commandService.ExecuteAsync(commandContext, argPos, _services);
        }

        private string? GetErrorResponse(IResult result)
        {
            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    return null;

                case CommandError.ParseFailed:
                    return _localizer["errors.parseFailed"];

                case CommandError.BadArgCount:
                    return _localizer["errors.invalidNumberOfArguments"];

                case CommandError.UnmetPrecondition:
                    return _localizer[result.ErrorReason];

                case CommandError.Exception:
                    var executeResult = (ExecuteResult) result;
                    var exception = executeResult.Exception;

                    return exception switch
                    {
                        InvalidUserLimitException => _localizer["errors.invalidUserLimit"],
                        InvalidVoiceChannelIdentifierException => _localizer["errors.invalidVoiceChannelIdentifier"],
                        VoiceChannelNotFoundException => _localizer["errors.voiceChannelNotFound"],
                        _ => _localizer["errors.unknown"]
                    };

                default:
                    return _localizer["errors.unknown"];
            }
        }
    }
}
