using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;
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
    public class LockModule : ModuleBase<SocketCommandContext>
    {
        [Command("lock")]
        public async Task Lock()
        {
            var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = voiceChannel.Users.Count);
            Log.Debug($"Locked channel {voiceChannel} for {voiceChannel.UserLimit} user(s).");
        }

        [Command("lock")]
        public async Task Lock(int userLimit)
        {
            var voiceChannel = ((SocketGuildUser) Context.User).VoiceChannel;

            // TODO: Find a way to use PreconditionAttribute
            if (!IsUsersLimitValid(userLimit, voiceChannel.Users.Count))
                throw new InvalidUserLimitException();

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = userLimit);
            Log.Debug($"Locked channel {voiceChannel} for {voiceChannel.UserLimit} user(s).");
        }

        private static bool IsUsersLimitValid(int value, int currentUsersCount)
        {
            const int minUsersLimit = 1;
            const int maxUsersLimit = 99;

            return value >= currentUsersCount
                && value is >= minUsersLimit and <= maxUsersLimit;
        }
    }
}