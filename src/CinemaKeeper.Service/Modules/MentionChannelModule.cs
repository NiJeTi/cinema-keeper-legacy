using System;
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
        public async Task MentionChannel()
        {
            await _shield.Protect(Context, async () =>
            {
                var user = Context.User;

                var voiceChannel = (user as SocketGuildUser)?.VoiceChannel
                    ?? throw new UserNotInVoiceChannelException();

                var usersList = voiceChannel.Users.Where(x => !x.Username.Equals(Context.User.Username));
                var channelMentionString = string.Join(Environment.NewLine, usersList.Select(x => x.Mention));

                await Context.Channel.SendMessageAsync(channelMentionString);

                Log.Debug($"Mentioned all users in {voiceChannel}.");
            });
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("castv")]
        public async Task MentionChannel([Remainder] string rawMention)
        {
            await _shield.Protect(Context, async () =>
            {
                var user = Context.User;
                var voiceChannels = Context.Guild.VoiceChannels!;

                SocketVoiceChannel voiceChannel = DefineMentionType(rawMention) switch
                {
                    MentionType.Id => voiceChannels.Single(x => x.Id.Equals(ulong.Parse(rawMention))),
                    MentionType.Wildcard => voiceChannels.First(x => Regex.IsMatch(x.Name, @$"^{rawMention}")),
                    _ => throw new WrongMentionException()
                };

                var usersList = voiceChannel.Users.Where(x => !x.Username.Equals(Context.User.Username));
                var channelMentionString = string.Join(Environment.NewLine, usersList.Select(x => x.Mention));

                await Context.Channel.SendMessageAsync(channelMentionString);

                Log.Debug($"Mentioned all users in {voiceChannel}.");
            });
        }

        private MentionType DefineMentionType(string rawMention)
        {
            if (Regex.IsMatch(rawMention, @"^\d{18}$"))
                return MentionType.Id;

            if (Regex.IsMatch(rawMention, @"^[\w\s]+$"))
                return MentionType.Wildcard;

            return default(MentionType);
        }
    }
}
