using System.Threading.Tasks;

using CinemaKeeper.Commands.Preconditions;
using CinemaKeeper.Services;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
[RequireVoiceChannel]
public class UnlockCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "unlock";

    private readonly ILogger _logger;
    private readonly ILocalizationProvider _localization;

    public UnlockCommand(
        ILogger logger,
        ILocalizationProvider localization)
    {
        _logger = logger.ForContext<UnlockCommand>();
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.unlock.definition.command");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .Build();
    }

    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.ManageChannels)]
    [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
    [RequireVoiceChannel]
    [SlashCommand(Command, "")]
    public async Task Execute()
    {
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;
        await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);

        var response = _localization.Get("commands.unlock.unlocked", voiceChannel.Mention);
        await RespondAsync(response, ephemeral: true);

        _logger.Debug("Unlocked channel {VoiceChannel}", voiceChannel.Name);
    }
}
