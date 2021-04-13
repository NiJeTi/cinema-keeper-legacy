using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Helpers;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    public class MentionChannelModule : ModuleBase<SocketCommandContext>
    {
        private readonly IExceptionShield<SocketCommandContext> _shield;

        public MentionChannelModule(IExceptionShield<SocketCommandContext> shield)
        {
            _shield = shield;
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("castv")]
        public async Task MentionChannel() =>
            await _shield.Protect(Context, async () =>
            {
                var user = Context.User;

                var voiceChannel = (user as SocketGuildUser)?.VoiceChannel
                    ?? throw new UserNotInVoiceChannelException();

                var usersList =
                    voiceChannel.Users.Where(x => !x.Username.Equals(user.Username, StringComparison.Ordinal));

                var channelMentionString = string.Join(Environment.NewLine, usersList.Select(x => x.Mention));

                await Context.Channel.SendMessageAsync(channelMentionString);

                Log.Debug($"Mentioned all users in {voiceChannel}.");
            }).ConfigureAwait(true);

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("castv")]
        public async Task MentionChannel([Remainder] string rawMention) =>
            await _shield.Protect(Context, async () =>
            {
                var voiceChannels = Context.Guild.VoiceChannels!;

                var voiceChannel = DefineMentionType(rawMention) switch
                {
                    MentionType.Id =>
                        voiceChannels.Single(x => x.Id.Equals(ulong.Parse(rawMention, CultureInfo.InvariantCulture))),
                    MentionType.Wildcard =>
                        voiceChannels.FirstOrDefault(x => Regex.IsMatch(x.Name, rawMention))
                        ?? throw new ChannelNotFoundException(),
                    _ => throw new WrongMentionException()
                };

                var usersList = voiceChannel.Users
                   .Where(x => !x.Username.Equals(Context.User.Username, StringComparison.Ordinal));

                var channelMentionString = string.Join(Environment.NewLine, usersList.Select(x => x.Mention));

                if (string.IsNullOrEmpty(channelMentionString))
                    return;

                await Context.Channel.SendMessageAsync(channelMentionString);

                Log.Debug($"Mentioned all users in {voiceChannel}.");
            }).ConfigureAwait(true);

        private static MentionType DefineMentionType(string rawMention) =>
            Regex.IsMatch(rawMention, @"^\d{18}$")
                ? MentionType.Id
                : Regex.IsMatch(rawMention, @"^[a-zA-Zа-яА-Я\s]+$")
                    ? MentionType.Wildcard
                    : default;
    }
}