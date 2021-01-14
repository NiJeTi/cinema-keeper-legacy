using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CinemaKeeper.Service.Services
{
    internal class DiscordLoggingService : BackgroundService
    {
        public DiscordLoggingService(IServiceProvider services)
        {
            var client = services.GetRequiredService<DiscordSocketClient>();
            var commandService = services.GetRequiredService<CommandService>();

            client.Log += Log;
            commandService.Log += Log;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Serilog.Log.Fatal(message.Exception, message.Message);

                    break;
                case LogSeverity.Error:
                    Serilog.Log.Error(message.Exception, message.Message);

                    break;
                case LogSeverity.Warning:
                    Serilog.Log.Warning(message.Exception, message.Message);

                    break;
                case LogSeverity.Info:
                    Serilog.Log.Information(message.Message);

                    break;
                case LogSeverity.Verbose:
                    Serilog.Log.Verbose(message.Message);

                    break;
                case LogSeverity.Debug:
                    Serilog.Log.Debug(message.Message);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.Severity), message.Severity, null);
            }

            return Task.CompletedTask;
        }
    }
}
