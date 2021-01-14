using System;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Helpers;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    internal class MentionChannelModule : ModuleBase<SocketCommandContext>
    {
        private readonly IExceptionShield<SocketCommandContext> _shield;

        public MentionChannelModule(IExceptionShield<SocketCommandContext> shield)
        {
            _shield = shield;
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("mentionChannel")]
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
    }
}
