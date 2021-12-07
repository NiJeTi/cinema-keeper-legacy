using System.Linq;

using CinemaKeeper.Persistence.Models;

using Microsoft.EntityFrameworkCore;

namespace CinemaKeeper.Persistence.Contexts.Quotes;

public class QuotesContext : DbContextBase, IQuotesContext
{
    public QuotesContext(string connectionString, int commandTimeout) : base(connectionString, commandTimeout) { }
    public QuotesContext(DbContextOptions options) : base(options) { }

    public DbSet<Quote> DbSet { get; set; }

    public IQueryable<Quote> Quotes => DbSet.AsQueryable();

    public void SaveQuote(Quote quote)
    {
        DbSet.Add(quote);
    }
}
