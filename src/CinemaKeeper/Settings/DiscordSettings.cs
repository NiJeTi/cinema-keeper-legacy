using System;

namespace CinemaKeeper.Settings;

[Serializable]
public record DiscordSettings
{
    public string BotToken { get; set; } = string.Empty;
    public ulong TestGuild { get; set; }
}
