using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace CinemaKeeper.Service.Modules
{
    public class UnlockModule : ModuleBase<SocketCommandContext>
    {
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("unlock")]
        public async Task Unlock()
        {
            var voiceChannel = (Context.User as SocketGuildUser)!.VoiceChannel;

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);
            await Context.Message.DeleteAsync();
        }
    }
}