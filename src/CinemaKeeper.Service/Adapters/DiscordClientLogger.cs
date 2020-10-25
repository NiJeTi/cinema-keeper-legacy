using System;
using System.Threading.Tasks;

using Discord;

using Serilog;

namespace CinemaKeeper.Service.Adapters
{
    internal static class DiscordClientLogger
    {
        public static Task LogMessage(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Log.Fatal(message.Exception, message.Message);

                    break;
                case LogSeverity.Error:
                    Log.Error(message.Exception, message.Message);

                    break;
                case LogSeverity.Warning:
                    Log.Warning(message.Exception, message.Message);

                    break;
                case LogSeverity.Info:
                    Log.Information(message.Message);

                    break;
                case LogSeverity.Verbose:
                    Log.Verbose(message.Message);

                    break;
                case LogSeverity.Debug:
                    Log.Debug(message.Message);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.Severity), message.Severity, null);
            }

            return Task.CompletedTask;
        }
    }
}