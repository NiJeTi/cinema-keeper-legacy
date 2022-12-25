using CinemaKeeper.Commands.Preconditions;
using CinemaKeeper.Extensions;
using CinemaKeeper.Services;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
public class LockCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "lock";

    private const int MinUserLimit = 1;
    private const int MaxUserLimit = 99;

    private readonly ILogger<LockCommand> _logger;
    private readonly ILocalizationProvider _localization;

    public LockCommand(
        ILogger<LockCommand> logger,
        ILocalizationProvider localization)
    {
        _logger = logger;
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.lock.definition.command");
        var limitOption = _localization.Get("commands.lock.definition.limitOption");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .AddOption(
                new SlashCommandOptionBuilder()
                   .WithName("limit")
                   .WithDescription(limitOption)
                   .WithType(ApplicationCommandOptionType.Integer)
                   .WithMinValue(MinUserLimit)
                   .WithMaxValue(MaxUserLimit))
           .Build();
    }

    [RequireVoiceChannel]
    [SlashCommand(Command, "")]
    public async Task Execute(int limit = default)
    {
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;
        limit = limit != default ? limit : voiceChannel.GetPresentUsers().Count;
        await voiceChannel.ModifyAsync(p => p.UserLimit = limit);

        var locked = _localization.Get("commands.lock.locked", voiceChannel.Mention, limit);
        await RespondAsync(locked, ephemeral: true);

        _logger.LogInformation("Locked channel \"{VoiceChannel}\" for {UserLimit} user(s)", voiceChannel.Name, limit);
    }
}
