using System;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Persistence;
using CinemaKeeper.Persistence.Models;

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
        private readonly AbobaContext _abobaContext;

        private readonly ILogger _logger;
        // private readonly IDbContextCreator<IQuotesContext> _contextCreator;

        public QuoteCommand(ILogger logger, AbobaContext abobaContext)
        {
            _logger = logger.ForContext<QuoteCommand>();
            _abobaContext = abobaContext;
            // _contextCreator = contextCreator;
        }

        [SlashCommand("quote", "Manage the most stunning quotes of the specified user")]
        public async Task HandleQuote(IUser user, string? message = null)
        {
            // using var context = _contextCreator.Create();
            await Context.Interaction.DeferAsync();

            if (message is not null)
            {
                // _abobaContext.Quotes.Add(new Quote
                // {
                //     Id = Guid.NewGuid(),
                //     Text = message,
                //     CreateDate = Context.Interaction.CreatedAt.UtcDateTime,
                //     AuthorId = Context.User.Id,
                //     UserId = user.Id
                // });
                //
                // await _abobaContext.SaveChangesAsync();

                await Context.Interaction.ModifyOriginalResponseAsync(properties =>
                {
                    properties.Content = "Quote created";
                });
            }
            else
            {
                // var quotes = _abobaContext.Quotes.Where(x => x.UserId == user.Id)
                //    .Select(x => $"[{x.CreateDate.ToShortDateString()}] {x.Text}");
                //
                // await Context.Channel.SendMessageAsync(string.Join("\n", quotes));

                await Context.Interaction.ModifyOriginalResponseAsync(properties =>
                {
                    properties.Content = "Done";
                });
            }
            // Console.WriteLine(context.Quotes.Count());
        }

        [SlashCommand("quote2", "Manage the most stunning quotes of the specified user")]
        public async Task HandleQuote2(IUser user, string? message = null)
        {
            // using var context = _contextCreator.Create();
            await Context.Interaction.DeferAsync();

            if (message is not null)
            {
                _abobaContext.Quotes.Add(new Quote
                {
                    Id = Guid.NewGuid(),
                    Text = message,
                    CreateDate = Context.Interaction.CreatedAt.UtcDateTime,
                    AuthorId = Context.User.Id,
                    UserId = user.Id
                });

                await _abobaContext.SaveChangesAsync();

                await Context.Interaction.ModifyOriginalResponseAsync(properties =>
                {
                    properties.Content = "Quote created";
                });
            }
            else
            {
                var quotes = _abobaContext.Quotes.Where(x => x.UserId == user.Id)
                   .Select(x => $"[{x.CreateDate.ToShortDateString()}] {x.Text}");

                await Context.Channel.SendMessageAsync(string.Join("\n", quotes));

                await Context.Interaction.ModifyOriginalResponseAsync(properties =>
                {
                    properties.Content = "Done";
                });
            }
            // Console.WriteLine(context.Quotes.Count());
        }
    }
}
