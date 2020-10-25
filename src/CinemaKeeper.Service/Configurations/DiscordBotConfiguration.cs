using System;

namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    public class DiscordBotConfiguration
    {
        public string Token { get; set; } = string.Empty;
    }
}