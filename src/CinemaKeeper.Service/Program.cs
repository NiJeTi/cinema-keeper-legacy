using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CinemaKeeper.Service.Configurations;
using CinemaKeeper.Service.Extensions;
using CinemaKeeper.Service.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace CinemaKeeper.Service
{
    [SuppressMessage("Reliability", "CA2000", Scope = "type", Target = "~T:CinemaKeeper.Service.Program")]
    internal static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHost(args).Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .UseSerilog((hostContext, _) =>
                    Log.Logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(hostContext.Configuration)
                       .CreateLogger(), true)
               .ConfigureServices((hostContext, services) =>
                {
                    services.AddConfigurations(hostContext.Configuration);

                    var discordBotConfiguration = (services
                       .Single(s => s.ServiceType == typeof(DiscordBotConfiguration))
                       .ImplementationInstance as DiscordBotConfiguration)!;

                    services
                       .AddDiscordClient(discordBotConfiguration)
                       .AddCommandService(discordBotConfiguration)
                       .AddLocalization();

                    services.AddHostedService<BotService>();
                    services.AddHostedService<DiscordLoggingService>();
                })
               .Build();
    }
}