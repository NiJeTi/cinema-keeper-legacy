using System.Threading.Tasks;

using CinemaKeeper.Storage.Models;

namespace CinemaKeeper.Storage;

public interface IAsyncDatabaseWriter
{
    Task AddQuoteAsync(Quote quote);
}
