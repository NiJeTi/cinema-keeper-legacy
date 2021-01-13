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
            try
            {
                await action();
            }
            catch (UserNotInVoiceChannelException)
            {
                await context.Channel.SendMessageAsync("User must be in a voice channel to use this command.");
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }
    }
}