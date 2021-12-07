using System;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Persistence;
using CinemaKeeper.Persistence.Contexts.Quotes;

using Discord;
using Discord.Commands;
using Discord.Interactions;

using Serilog;

namespace CinemaKeeper.Commands
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(GuildPermission.SendMessages)]
    [RequireBotPermission(GuildPermission.UseApplicationCommands)]
    public class QuoteCommand : InteractionModuleBase
    {
        private readonly ILogger _logger;
        private readonly IDbContextCreator<IQuotesContext> _contextCreator;

        public QuoteCommand(ILogger logger, IDbContextCreator<IQuotesContext> contextCreator)
        {
            _logger = logger.ForContext<QuoteCommand>();
            _contextCreator = contextCreator;
        }

        [SlashCommand("quote", "Manage the most stunning quotes of the specified user")]
        public async Task HandleQuote(IUser user, string? message = null)
        {
            // using var context = _contextCreator.Create();
            Console.WriteLine(message);
            // Console.WriteLine(context.Quotes.Count());
        }
    }
}
