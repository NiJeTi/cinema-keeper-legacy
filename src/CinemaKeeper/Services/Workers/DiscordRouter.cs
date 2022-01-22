using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CinemaKeeper.Commands;
using CinemaKeeper.Exceptions;
using CinemaKeeper.Extensions;
using CinemaKeeper.Settings;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Serilog;

namespace CinemaKeeper.Services.Workers;

public class DiscordRouter : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IOptions<DiscordSettings> _discordSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly ILocalizationProvider _localization;

    public DiscordRouter(
        ILogger logger,
        IOptions<DiscordSettings> discordSettings,
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        DiscordSocketClient client,
        InteractionService interactionService,
        IDiscordLogger discordLogger,
        ILocalizationProvider localization)
    {
        _logger = logger.ForContext<DiscordRouter>();
        _discordSettings = discordSettings;
        _serviceProvider = serviceProvider;
        _environment = environment;
        _client = client;
        _interactionService = interactionService;
        _localization = localization;

        client.Ready += ClientReady;
        client.SlashCommandExecuted += SlashCommandExecuted;
        interactionService.SlashCommandExecuted += AfterSlashCommandExecuted;

        client.Log += m => discordLogger.Log(client, m);
        interactionService.Log += m => discordLogger.Log(interactionService, m);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        await _client.LoginAsync(TokenType.Bot, _discordSettings.Value.BotToken);
        await _client.StartAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        await DeleteCommands();

        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    public override void Dispose()
    {
        base.Dispose();

        _client.Dispose();
        _interactionService.Dispose();

        GC.SuppressFinalize(this);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    private async Task ClientReady()
    {
        using var scope = _serviceProvider.CreateScope();

        Func<SlashCommandProperties, Task> registerCommand = _environment.IsDevelopment()
            ? command => _client.GetGuild(_discordSettings.Value.TestGuild).CreateApplicationCommandAsync(command)
            : command => _client.CreateGlobalApplicationCommandAsync(command);

        var registrationTasks = Assembly.GetExecutingAssembly().GetTypes()
           .Where(t =>
                typeof(ISlashCommandBuilder).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract)
           .Select(t => (ISlashCommandBuilder) ActivatorUtilities.CreateInstance(scope.ServiceProvider, t))
           .Select(b => registerCommand(b.Build()))
           .ToList();

        await Task.WhenAll(registrationTasks);
    }

    private async Task DeleteCommands()
    {
        var commands = _environment.IsDevelopment()
            ? await _client.GetGuild(_discordSettings.Value.TestGuild).GetApplicationCommandsAsync()
            : await _client.GetGlobalApplicationCommandsAsync();

        await Task.WhenAll(commands.Select(c => c.DeleteAsync()));
    }

    private async Task SlashCommandExecuted(SocketSlashCommand command)
    {
        var commandContext = new SocketInteractionContext(_client, command);
        await _interactionService.ExecuteCommandAsync(commandContext, _serviceProvider);
    }

    private async Task AfterSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
    {
        _logger.Information("Executed command \"{Command}\" for user \"{User}\"",
            command.Name, context.User.GetFullUsername());

        if (!result.IsSuccess)
        {
            string response;

            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    response = _localization.Get(result.ErrorReason);

                    break;

                case InteractionCommandError.Exception:
                    var exception = ((ExecuteResult) result).Exception;

                    if (exception is FlowException)
                        response = _localization.Get(exception.Message);
                    else
                        goto default;

                    break;

                default:
                    response = _localization.Get("errors.unknown");

                    break;
            }

            try
            {
                await context.Interaction.RespondAsync(response, ephemeral: true);
            }
            catch
            {
                await context.Interaction.ModifyOriginalResponseAsync(m => m.Content = response);
            }
        }
    }
}
