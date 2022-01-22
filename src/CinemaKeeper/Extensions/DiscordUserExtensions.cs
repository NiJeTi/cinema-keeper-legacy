using Discord;

namespace CinemaKeeper.Extensions;

public static class DiscordUserExtensions
{
    public static string GetFullUsername(this IUser user) => $"{user.Username}#{user.Discriminator}";
}
