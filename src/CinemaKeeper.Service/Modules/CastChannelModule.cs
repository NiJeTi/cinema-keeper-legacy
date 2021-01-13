using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Serilog;

namespace CinemaKeeper.Service.Modules
{
	public class CastChannelModule : ModuleBase<SocketCommandContext>
	{
		[RequireContext(ContextType.Guild)]
		[RequireBotPermission(GuildPermission.ManageChannels | GuildPermission.ManageMessages)]
		[RequireUserPermission(GuildPermission.Connect | GuildPermission.Speak)]
		[Command("castChannel")]
		public async Task CastChannel()
		{
			var user         = Context.User;
			var voiceChannel = (user as SocketGuildUser)?.VoiceChannel;

			if (voiceChannel is null)
			{
				await Context.Channel.SendMessageAsync("User must be in a voice channel to use this command.");

				return;
			}

			var usersList            = voiceChannel.Users.Where(x => !x.Username.Equals(Context.User.Username));
			var channelMentionString = string.Join(" ", usersList.Select(x => x.Mention));

			await Context.Channel.SendMessageAsync(channelMentionString);

			Log.Debug($"Mentioned all users in {voiceChannel}.");
		}
	}
}