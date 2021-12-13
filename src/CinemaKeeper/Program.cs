using System;
using System.Reflection;

using CinemaKeeper.Extensions;
using CinemaKeeper.Services;
using CinemaKeeper.Services.Workers;
using CinemaKeeper.Settings;
using CinemaKeeper.Storage;

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

namespace CinemaKeeper;

internal static class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

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
           .ConfigureLogging(builder => builder.ClearProviders())
           .UseSerilog((context, configuration) =>
            {
                configuration
                   .ReadFrom.Configuration(context.Configuration);
            })
           .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddDbContext<PostgresContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("Postgres"));
                });

                services.Configure<DiscordSettings>(configuration.GetSection("Discord"));

                services.AddAutoMapper(options => options.AddProfile<DtoMappingProfile>());

                services.AddSingleton<ILocalizationProvider, LocalizationProvider>();

                services.AddSingleton<DiscordSocketClient>();

                services.AddSingleton(provider =>
                {
                    var commandService = new CommandService();
                    commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), provider);

                    return commandService;
                });

                services.AddSingleton(provider =>
                {
                    var interactionService =
                        new InteractionService(provider.GetRequiredService<DiscordSocketClient>());

                    interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(),
                        provider.CreateScope().ServiceProvider);

                    return interactionService;
                });

                services.AddHostedService<BotService>();
                services.AddHostedService<DiscordLoggingService>();
            })
           .Build();
}
