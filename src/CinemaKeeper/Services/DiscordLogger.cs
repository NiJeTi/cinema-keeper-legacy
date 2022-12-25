using Discord;

namespace CinemaKeeper.Services;

public class DiscordLogger : IDiscordLogger
{
    private readonly ILoggerProvider _loggerProvider;

    public DiscordLogger(ILoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
    }

    public Task Log(object source, LogMessage message)
    {
        var logger = _loggerProvider.CreateLogger(source.GetType().ToString());

        switch (message.Severity)
        {
            case LogSeverity.Critical:
                logger.LogCritical(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Error:
                logger.LogError(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Warning:
                logger.LogWarning(message.Exception, "{Message}", message.Message);

                break;

            case LogSeverity.Info:
                logger.LogInformation("{Message}", message.Message);

                break;

            case LogSeverity.Debug:
                logger.LogDebug("{Message}", message.Message);

                break;

            case LogSeverity.Verbose:
                logger.LogTrace("{Message}", message.Message);

                break;
        }

        return Task.CompletedTask;
    }
}
