using System.Collections.Generic;
using System.Linq;

using Discord.WebSocket;

namespace CinemaKeeper.Extensions;

public static class SocketGuildChannelExtensions
{
    public static IReadOnlyCollection<SocketGuildUser> GetPresentUsers(this SocketGuildChannel channel)
    {
        return channel.Users.Where(x => x.VoiceChannel == channel).ToList();
    }
}
