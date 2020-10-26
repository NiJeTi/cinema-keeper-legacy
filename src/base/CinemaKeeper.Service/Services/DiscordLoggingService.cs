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
            var client = services.GetService<DiscordSocketClient>();
            var commandService = services.GetService<CommandService>();

            client.Log += Log;
            commandService.Log += Log;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
                await Task.Yield();
        }

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