using System.Threading.Tasks;

using Discord.Commands;

namespace CinemaKeeper.Service.Modules
{
    public class LockModule : ModuleBase<SocketCommandContext>
    {
        [Command("lock")]
        public Task Lock() => ReplyAsync("Command *lock* is not implemeted yet");
    }
}