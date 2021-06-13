using System;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

namespace CinemaKeeper.Service.Preconditions
{
    public class UserMustBeInVoiceChannelAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var user = (SocketGuildUser) context.User;

            return Task.FromResult(user.VoiceChannel != null
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("errors.userMustBeInVoiceChannel"));
        }
    }
}