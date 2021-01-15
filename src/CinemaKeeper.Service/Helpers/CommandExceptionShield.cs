using System;
using System.Threading.Tasks;

using CinemaKeeper.Service.Exceptions;

using Discord.Commands;

using Serilog;

namespace CinemaKeeper.Service.Helpers
{
    public class CommandExceptionShield : IExceptionShield<SocketCommandContext>
    {
        public async Task Protect(SocketCommandContext context, Func<Task> action)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                await action();
            }
            catch (UserNotInVoiceChannelException)
            {
                await context.Channel.SendMessageAsync("User must be in a voice channel to use this command. Enter voice chat or use echanced version of command.");
            }
            catch (WrongMentionException)
            {
                await context.Channel.SendMessageAsync("Wrong mention. Use voice channel ID or type it's name.");
            }
            catch (Exception e)
            {
                Log.Warning(e, "Unexpected Error.");
            }
        }
    }
}
