using Discord;

namespace CinemaKeeper.SlashCommands;

public interface ISlashCommandCreator
{
    SlashCommandProperties GetSlashCommand();
    SlashCommandProperties GetTestSlashCommand();
}
