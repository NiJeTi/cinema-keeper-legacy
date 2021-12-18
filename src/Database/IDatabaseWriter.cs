using CinemaKeeper.Database.Models;

namespace CinemaKeeper.Database;

public interface IDatabaseWriter
{
    void AddQuote(Quote quote);
}
