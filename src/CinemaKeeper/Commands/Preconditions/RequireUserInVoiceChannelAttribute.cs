using System;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

namespace CinemaKeeper.Commands.Preconditions;

public class UserMustBeInVoiceChannelAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(
        ICommandContext context,
        CommandInfo command,
        IServiceProvider services) =>
        Task.FromResult(((SocketGuildUser) context.User).VoiceChannel is not null
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("errors.userMustBeInVoiceChannel"));
}
