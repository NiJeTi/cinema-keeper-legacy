using System;

namespace CinemaKeeper.Settings;

[Serializable]
public record DiscordSettings
{
    public string BotToken { get; set; } = string.Empty;

    public string CommandPrefix { get; set; } = "!";
}
