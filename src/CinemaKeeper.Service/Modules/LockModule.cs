using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;
using CinemaKeeper.Service.Helpers;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    public class LockModule : ModuleBase<SocketCommandContext>
    {
        private readonly IExceptionShield<SocketCommandContext> _shield;

        public LockModule(IExceptionShield<SocketCommandContext> shield)
        {
            _shield = shield;
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("lock")]
        public async Task Lock()
        {
            await _shield.Protect(Context, async () =>
            {
                var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel
                    ?? throw new UserNotInVoiceChannelException();

                await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = voiceChannel.Users.Count);

                Log.Debug($"Locked channel {voiceChannel} for {voiceChannel.UserLimit} user(s).");
            });
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("lock")]
        public async Task Lock([Remainder] string usersLimitRaw)
        {
            await _shield.Protect(Context, async () =>
            {
                var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel
                    ?? throw new UserNotInVoiceChannelException();

                var textChannel = Context.Channel;

                if (!int.TryParse(usersLimitRaw, out var usersLimit))
                {
                    await textChannel.SendMessageAsync("Lock user limit must be an integer.");

                    return;
                }

                if (!IsUsersLimitValid(usersLimit, voiceChannel.Users.Count))
                {
                    await textChannel.SendMessageAsync("Lock user limit has invalid value.");

                    return;
                }

                await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = usersLimit);

                Log.Debug($"Locked channel {voiceChannel} for {voiceChannel.UserLimit} user(s).");
            });
        }

        private static bool IsUsersLimitValid(int value, int currentUsersCount)
        {
            const int minUsersLimit = 1;
            const int maxUsersLimit = 99;

            return value >= currentUsersCount
                && value >= minUsersLimit
                && value <= maxUsersLimit;
        }
    }
}
