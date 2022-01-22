using CinemaKeeper.Storage.Models;

namespace CinemaKeeper.Storage;

public interface IDatabaseWriter
{
    void AddQuote(Quote quote);
}
