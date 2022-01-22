using System.Threading.Tasks;

using Discord;

namespace CinemaKeeper.Services;

public interface IDiscordLogger
{
    Task Log(object source, LogMessage message);
}
