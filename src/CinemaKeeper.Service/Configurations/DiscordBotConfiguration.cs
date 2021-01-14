using System;

namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    internal record DiscordBotConfiguration
    {
        public string Token { get; set; } = string.Empty;

        public string Prefix { get; set; } = "!";
    }
}
