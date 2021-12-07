namespace CinemaKeeper.Persistence.Contexts.Quotes;

public class QuotesContextCreator : DbContextCreator<IQuotesContext>
{
    public QuotesContextCreator(string connectionString, int commandTimeout) :
        base(connectionString, commandTimeout) { }

    protected override IQuotesContext Create(string connectionString, int commandTimeout) =>
        new QuotesContext(connectionString, commandTimeout);
}
