using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Exceptions;
using CinemaKeeper.Extensions;
using CinemaKeeper.Services;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.SendMessages)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
public class CastCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "cast";

    private readonly ILogger<CastCommand> _logger;
    private readonly ILocalizationProvider _localization;

    public CastCommand(
        ILogger<CastCommand> logger,
        ILocalizationProvider localization)
    {
        _logger = logger;
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.cast.definition.command");
        var channelOption = _localization.Get("commands.cast.definition.channelOption");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .AddOption(
                new SlashCommandOptionBuilder()
                   .WithName("channel")
                   .WithDescription(channelOption)
                   .WithType(ApplicationCommandOptionType.Channel)
                   .AddChannelType(ChannelType.Voice))
           .Build();
    }

    [SlashCommand(Command, "")]
    public async Task Execute(IGuildChannel? channel = null)
    {
        var currentUser = (SocketGuildUser) Context.User;

        var guildChannel = (SocketGuildChannel) (channel
            ?? currentUser.VoiceChannel
            ?? throw new UserNotInVoiceChannelException());

        if (!guildChannel.GetPresentUsers().Except(new[] { currentUser }).Any())
        {
            var noUsers = _localization.Get("commands.cast.noUsers", guildChannel);
            await RespondAsync(noUsers, ephemeral: true);
        }
        else
        {
            var mention = BuildMentionString(guildChannel.GetPresentUsers(), currentUser);
            await RespondAsync(mention, allowedMentions: AllowedMentions.All);

            _logger.LogDebug("Mentioned all users in \"{VoiceChannel}\"", guildChannel.Name);
        }
    }

    private static string BuildMentionString(IEnumerable<SocketGuildUser> channelUsers, IUser initiator)
    {
        var mentions = channelUsers
           .Where(u => !u.Username.Equals(initiator.Username, StringComparison.Ordinal))
           .Select(u => u.Mention);

        return string.Join(Environment.NewLine, mentions);
    }
}
