using System.Threading.Tasks;

using CinemaKeeper.Commands.Preconditions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(GuildPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
[UserMustBeInVoiceChannel]
public class UnlockCommand : ModuleBase<SocketCommandContext>
{
    [Command("unlock")]
    public async Task Unlock()
    {
        var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;
        await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = null);

        Log.Debug("Unlocked channel {VoiceChannel}", voiceChannel.Name);
    }
}
