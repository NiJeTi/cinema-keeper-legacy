using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Exceptions;
using CinemaKeeper.Services;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(ChannelPermission.SendMessages)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
public class CastCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "cast";

    private readonly ILogger _logger;
    private readonly ILocalizationProvider _localization;

    public CastCommand(
        ILogger logger,
        ILocalizationProvider localization)
    {
        _logger = logger.ForContext<CastCommand>();
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.cast.definition.command");
        var channelOption = _localization.Get("commands.cast.definition.channelOption");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .AddOption("channel", ApplicationCommandOptionType.Channel, channelOption,
                channelTypes: new List<ChannelType> { ChannelType.Voice })
           .Build();
    }

    [SlashCommand(Command, "")]
    public async Task Execute(IGuildChannel? channel = null)
    {
        var user = (SocketGuildUser) Context.User;

        var guildChannel = (SocketGuildChannel) (channel
            ?? user.VoiceChannel
            ?? throw new UserNotInVoiceChannelException());

        if (!guildChannel.Users.Any())
        {
            var noUsers = _localization.Get("commands.cast.noUsers", guildChannel);
            await RespondAsync(noUsers, ephemeral: true);
        }
        else
        {
            var mention = BuildMentionString(guildChannel.Users, user);
            await RespondAsync(mention, allowedMentions: AllowedMentions.All);

            _logger.Debug("Mentioned all users in {VoiceChannel}", guildChannel.Name);
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
