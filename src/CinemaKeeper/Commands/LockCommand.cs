using System.Threading.Tasks;

using CinemaKeeper.Commands.Preconditions;
using CinemaKeeper.Commands.Preconditions.Parameters;
using CinemaKeeper.Services;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
public class LockCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "lock";

    public const int MinUserLimit = 1;
    public const int MaxUserLimit = 99;

    private readonly ILocalizationProvider _localization;

    private readonly ILogger _logger;

    public LockCommand(
        ILogger logger,
        ILocalizationProvider localization)
    {
        _logger = logger.ForContext<LockCommand>();
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.lock.definition.command");
        var limitOption = _localization.Get("commands.lock.definition.limitOption");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .AddOption("limit", ApplicationCommandOptionType.Integer, limitOption,
                minValue: MinUserLimit, maxValue: MaxUserLimit)
           .Build();
    }

    [RequireVoiceChannel]
    [SlashCommand(Command, "")]
    public async Task Execute([UserLimitParameter] int limit = default)
    {
        // BUG: Voice channel is not null after leave for several seconds
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;
        limit = limit != default ? limit : voiceChannel.Users.Count;
        await voiceChannel.ModifyAsync(p => p.UserLimit = limit);

        var locked = _localization.Get("commands.lock.locked", voiceChannel.Mention, limit);
        await RespondAsync(locked, ephemeral: true);

        _logger.Debug("Locked channel \"{VoiceChannel}\" for {UserLimit} user(s)", voiceChannel.Name, limit);
    }
}
