using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Service.Helpers;
using CinemaKeeper.Service.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(GuildPermission.SendMessages)]
    public class MentionVoiceChannelModule : ModuleBase<SocketCommandContext>
    {
        [RequireUserPermission(GuildPermission.Connect)]
        [UserMustBeInVoiceChannel]
        [Command("mentv")]
        public async Task MentionVoiceChannel()
        {
            var user = (SocketGuildUser) Context.User;
            var voiceChannel = user.VoiceChannel;

            await Context.Channel.SendMessageAsync(BuildMentionString(user, voiceChannel.Users));
            Log.Debug($"Mentioned all users in {voiceChannel}.");
        }

        [Command("mentv")]
        public async Task MentionVoiceChannel([Remainder] string rawId)
        {
            var user = (SocketGuildUser) Context.User;

            var allVoiceChannels = Context.Guild.VoiceChannels;
            var idType = VoiceChannelIdentifier.IdentifyType(rawId);
            var mentionedVoiceChannel = VoiceChannelIdentifier.Identify(allVoiceChannels, idType, rawId);

            await Context.Channel.SendMessageAsync(BuildMentionString(user, mentionedVoiceChannel.Users));
            Log.Debug($"Mentioned all users in {mentionedVoiceChannel}.");
        }

        private static string BuildMentionString(SocketGuildUser mentioner,
            IEnumerable<SocketGuildUser> voiceChannelUsers)
        {
            var mentionedUsers = voiceChannelUsers
               .Where(u => !u.Username.Equals(mentioner.Username, StringComparison.Ordinal));

            return string.Join(Environment.NewLine, mentionedUsers.Select(x => x.Mention));
        }
    }
}
