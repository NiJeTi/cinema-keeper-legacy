using System.Threading.Tasks;

using CinemaKeeper.Service.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(GuildPermission.ManageChannels)]
    [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
    [UserMustBeInVoiceChannel]
    public class UnlockModule : ModuleBase<SocketCommandContext>
    {
        [Command("unlock")]
        public async Task Unlock()
        {
            var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);
            Log.Debug($"Unlocked channel {voiceChannel}.");
        }
    }
}
