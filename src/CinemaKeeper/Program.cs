using System.Reflection;

using CinemaKeeper.Services;
using CinemaKeeper.Services.Workers;
using CinemaKeeper.Settings;
using CinemaKeeper.Storage.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Serilog;

using ILogger = Serilog.ILogger;

namespace CinemaKeeper;

internal static class Program
{
    private static void Main(string[] args)
    {
        var logger = new LoggerConfiguration()
           .WriteTo.Console()
           .CreateBootstrapLogger() as ILogger;

        try
        {
            var host = CreateHost(args);

            var serviceProvider = host.Services;
            logger = serviceProvider.GetRequiredService<ILogger>();

            host.Run();
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHost CreateHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
           .ConfigureLogging(builder => builder.ClearProviders())
           .UseSerilog(
                (context, configuration) =>
                {
                    configuration
                       .ReadFrom.Configuration(context.Configuration);
                })
           .ConfigureServices(
                (context, services) =>
                {
                    var configuration = context.Configuration;

                    services.Configure<DiscordSettings>(configuration.GetSection("Discord"));
                    services.Configure<LocalizationSettings>(configuration.GetSection("Localization"));

                    services.AddSingleton<ILocalizationProvider, LocalizationProvider>();

                    services.AddDiscordClient();
                    services.ConfigurePersistentStorage(configuration);

                    services.AddHostedService<DiscordRouter>();
                })
           .Build();

    private static void AddDiscordClient(this IServiceCollection services)
    {
        services.AddSingleton<IDiscordLogger, DiscordLogger>();

        services.AddSingleton(
            _ =>
            {
                var config = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds
                        | GatewayIntents.GuildPresences
                        | GatewayIntents.GuildMembers
                        | GatewayIntents.GuildVoiceStates
                };

                return new DiscordSocketClient(config);
            });

        services.AddSingleton(
            provider =>
            {
                var discordClient = provider.GetRequiredService<DiscordSocketClient>();
                var interactionService = new InteractionService(discordClient);

                using var scope = provider.CreateScope();

                interactionService
                   .AddModulesAsync(Assembly.GetExecutingAssembly(), scope.ServiceProvider)
                   .Wait();

                return interactionService;
            });
    }
}
