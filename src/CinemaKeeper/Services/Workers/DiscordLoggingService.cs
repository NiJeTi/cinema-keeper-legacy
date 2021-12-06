using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Hosting;

using Serilog;

namespace CinemaKeeper.Services.Workers;

internal class DiscordLoggingService : BackgroundService
{
    private readonly ILogger _logger;

    public DiscordLoggingService(
        ILogger logger,
        DiscordSocketClient client,
        CommandService commandService)
    {
        _logger = logger.ForContext<DiscordLoggingService>();

        client.Log += Log;
        commandService.Log += Log;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    private Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                _logger.Fatal(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Error:
                _logger.Error(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Warning:
                _logger.Warning(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Info:
                _logger.Information("{Message}", message.Message);

                break;

            case LogSeverity.Verbose:
                _logger.Verbose("{Message}", message.Message);

                break;

            case LogSeverity.Debug:
                _logger.Debug("{Message}", message.Message);

                break;
        }

        return Task.CompletedTask;
    }
}
