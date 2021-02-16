using System;


namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    public record DiscordBotConfiguration
    {
        public string Token { get; set; } = string.Empty;

        public string Prefix { get; set; } = "!";

        public ServerManager ServerManager { get; set; } = new ServerManager(); 
    }
}
