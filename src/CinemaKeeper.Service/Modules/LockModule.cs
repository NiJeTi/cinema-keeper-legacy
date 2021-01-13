using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
    public class LockModule : ModuleBase<SocketCommandContext>
    {
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("lock")]
        public async Task Lock()
        {
            var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;

            if (voiceChannel is null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel to use this command.");

                return;
            }

            await voiceChannel.ModifyAsync(vcp => vcp.UserLimit = voiceChannel.Users.Count);

            Log.Debug($"Locked channel {voiceChannel} for {voiceChannel.UserLimit} user(s).");
        }

        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
        [Command("lock")]
        public async Task Lock([Remainder] string usersLimitRaw)
        {
            var voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;
            var textChannel = Context.Channel;

            if (voiceChannel is null)
            {
                await textChannel.SendMessageAsync("User must be in a voice channel to use this command.");

                return;
            }

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