using System;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CinemaKeeper.Commands.Preconditions;

public class RequireVoiceChannelAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services) =>
        Task.FromResult(((SocketGuildUser) context.User).VoiceChannel is not null
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("errors.userMustBeInVoiceChannel"));
}
