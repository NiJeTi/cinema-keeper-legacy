using System.Collections.ObjectModel;

using Discord.WebSocket;

namespace CinemaKeeper.Extensions;

public static class SocketGuildChannelExtensions
{
    public static ReadOnlyCollection<SocketGuildUser> GetPresentUsers(this SocketGuildChannel channel)
    {
        return channel.Users.Where(x => x.VoiceChannel == channel).ToList().AsReadOnly();
    }
}
