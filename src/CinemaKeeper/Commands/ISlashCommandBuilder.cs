using Discord;

namespace CinemaKeeper.Commands;

public interface ISlashCommandBuilder
{
    SlashCommandProperties Build();
}
