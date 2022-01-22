using System;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;

namespace CinemaKeeper.Commands.Preconditions.Parameters;

public class UserLimitParameter : ParameterPreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        IParameterInfo parameterInfo,
        object value,
        IServiceProvider services)
    {
        if (value is not int limit)
            return Task.FromResult(PreconditionResult.FromSuccess());

        if (!parameterInfo.IsRequired && value == parameterInfo.DefaultValue)
            return Task.FromResult(PreconditionResult.FromSuccess());

        return limit is < LockCommand.MinUserLimit or > LockCommand.MaxUserLimit
            ? Task.FromResult(PreconditionResult.FromError("errors.invalidUserLimit"))
            : Task.FromResult(PreconditionResult.FromSuccess());
    }
}
