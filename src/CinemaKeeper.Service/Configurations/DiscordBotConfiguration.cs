using System;

using Discord;

namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    internal class DiscordBotConfiguration
    {
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        public string Token { get; set; } = string.Empty;

        public int Permissions { get; set; } = 3152;
    }
}