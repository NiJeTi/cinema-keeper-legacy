using System;

using CinemaKeeper.Service.Extensions;
using CinemaKeeper.Service.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace CinemaKeeper.Service
{
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
                    services.AddDiscordBotConfiguration(hostContext.Configuration);

                    services.AddHostedService<BotService>();
                })
               .Build();
    }
}