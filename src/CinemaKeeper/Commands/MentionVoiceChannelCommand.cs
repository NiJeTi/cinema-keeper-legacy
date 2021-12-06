using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Commands.Helpers;
using CinemaKeeper.Commands.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(GuildPermission.SendMessages)]
public class MentionVoiceChannelCommand : ModuleBase<SocketCommandContext>
{
    private readonly ILogger _logger;

    public MentionVoiceChannelCommand(ILogger logger)
    {
        _logger = logger.ForContext<MentionVoiceChannelCommand>();
    }

    [RequireUserPermission(GuildPermission.Connect)]
    [UserMustBeInVoiceChannel]
    [Command("cast")]
    public async Task MentionVoiceChannel()
    {
        var user = (SocketGuildUser) Context.User;
        var voiceChannel = user.VoiceChannel;

        await Context.Channel.SendMessageAsync(BuildMentionString(user, voiceChannel.Users));
        _logger.Debug("Mentioned all users in {VoiceChannel}", voiceChannel.Name);
    }

    [Command("cast")]
    public async Task MentionVoiceChannel([Remainder] string rawId)
    {
        var user = (SocketGuildUser) Context.User;

        var allVoiceChannels = Context.Guild.VoiceChannels;
        var idType = VoiceChannelIdentifier.IdentifyType(rawId);
        var voiceChannel = VoiceChannelIdentifier.Identify(allVoiceChannels, idType, rawId);

        await Context.Channel.SendMessageAsync(BuildMentionString(user, voiceChannel.Users));
        _logger.Debug("Mentioned all users in {VoiceChannel}", voiceChannel.Name);
    }

    private static string BuildMentionString(
        SocketGuildUser initiator,
        IEnumerable<SocketGuildUser> voiceChannelUsers)
    {
        var mentionedUsers = voiceChannelUsers
           .Where(u => !u.Username.Equals(initiator.Username, StringComparison.Ordinal));

        return string.Join(Environment.NewLine, mentionedUsers.Select(u => u.Mention));
    }
}
