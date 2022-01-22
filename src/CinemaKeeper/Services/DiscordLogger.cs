using System.Threading.Tasks;

using Discord;

using Serilog;

namespace CinemaKeeper.Services;

public class DiscordLogger : IDiscordLogger
{
    private readonly ILogger _logger;

    public DiscordLogger(ILogger logger)
    {
        _logger = logger;
    }

    public Task Log(object source, LogMessage message)
    {
        var logger = _logger.ForContext(source.GetType());

        switch (message.Severity)
        {
            case LogSeverity.Critical:
                logger.Fatal(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Error:
                logger.Error(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Warning:
                logger.Warning(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Info:
                logger.Information("{Message}", message.Message);

                break;

            case LogSeverity.Verbose:
                logger.Verbose("{Message}", message.Message);

                break;

            case LogSeverity.Debug:
                logger.Debug("{Message}", message.Message);

                break;
        }

        return Task.CompletedTask;
    }
}
