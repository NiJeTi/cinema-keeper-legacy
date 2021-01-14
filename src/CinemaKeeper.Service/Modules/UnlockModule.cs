using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Helpers;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    public class UnlockModule : ModuleBase<SocketCommandContext>
    {
        private readonly IExceptionShield<SocketCommandContext> _shield;

        public UnlockModule(IExceptionShield<SocketCommandContext> shield)
        {
            _shield = shield;
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("unlock")]
        public async Task Unlock()
        {
            await _shield.Protect(Context, async () =>
            {
                var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel
                    ?? throw new UserNotInVoiceChannelException();

                await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);

                Log.Debug($"Unlocked channel {voiceChannel}.");
            });
        }
    }
}
