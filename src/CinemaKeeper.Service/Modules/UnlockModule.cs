using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

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
            var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;
            
            if (voiceChannel is null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel to use this command.");
                
                return;
            }

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);

            Log.Debug($"Unlocked channel {voiceChannel}.");
        }
    }
}