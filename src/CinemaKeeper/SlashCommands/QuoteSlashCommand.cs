using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Persistence;
using CinemaKeeper.Storage;
using CinemaKeeper.Storage.Models;

using Discord;
using Discord.Commands;
using Discord.Interactions;

using Serilog;

namespace CinemaKeeper.SlashCommands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(GuildPermission.SendMessages)]
[RequireBotPermission(GuildPermission.UseApplicationCommands)]
public class QuoteSlashCommand : InteractionModuleBase, ISlashCommandCreator
{
    private readonly PostgresContext _postgresContext;
    private readonly ILogger _logger;

    public QuoteSlashCommand(ILogger logger, PostgresContext postgresContext)
    {
        _logger = logger.ForContext<QuoteSlashCommand>();
        _postgresContext = postgresContext;
    }

#if DEBUG
    [SlashCommand("quote-test", "Manage the most stunning quotes of the specified user")]
#else
    [SlashCommand("quote", "Manage the most stunning quotes of the specified user")]
#endif
    public async Task HandleQuote(IUser user, string? message = null)
    {
        await DeferAsync();
        var author = Context.User;

        if (message is not null)
        {
            CreateQuote(message, user.Id, author.Id, Context.Interaction.CreatedAt.UtcDateTime);
            SaveChanges();

            await SetResponseMessage("Quote created");
            _logger.Debug("Created command for {User} by {Author}", user.Username, author.Username);
        }
        else
        {
            var quotes = GetUserQuotes(user.Id);

            if (quotes.Length == 0)
            {
                await SetResponseMessage($"No quotes for {user.Mention}");
            }
            else
            {
                await SendMessageToOriginalChannel(await BuildQuotesMessage(user, quotes));
                await DeleteOriginalResponseAsync();
            }

            _logger.Debug("Displayed quotes for {User}", user.Username);
        }
    }

    private async Task<string> BuildQuotesMessage(IMentionable user, IEnumerable<Quote> quotes)
    {
        var header = $"Quotes for {user.Mention}";

        var quoteLines = new List<string>();

        foreach (var quote in quotes)
        {
            var author = await GetUser(quote.AuthorId);

            if (author is null)
            {
                _logger.Warning("Cannot find user {UserId}", quote.AuthorId);
            }

            quoteLines.Add(FormatQuote(quote.CreateDate, quote.Text, author?.Mention ?? "Unknown"));
        }

        return $"{header}\n\n{string.Join("\n", quoteLines)}";
    }

    private static string FormatQuote(DateTime createDate, string text, string authorMention) =>
        $"{createDate}: \"{text}\" by {authorMention}";

    private async Task<IUser?> GetUser(ulong id) => await Context.Guild.GetUserAsync(id);

    private async Task SendMessageToOriginalChannel(string message) => await ReplyAsync(message);

    private async Task SetResponseMessage(string message) => await FollowupAsync(message);

    private void CreateQuote(string message, ulong userId, ulong authorId, DateTime createDate)
    {
        var quote = new Quote(userId, message, createDate, authorId);

        _postgresContext.Add(quote);
    }

    private Quote[] GetUserQuotes(ulong userId) => _postgresContext.Quotes.Where(q => q.UserId == userId).ToArray();

    private void SaveChanges() => _postgresContext.SaveChanges();

    private static SlashCommandBuilder SlashCommandBuilder =>
        new SlashCommandBuilder()
           .WithDescription("Manage the most stunning quotes of the specified user")
           .AddOption("user", ApplicationCommandOptionType.User, "User who once told this", true)
           .AddOption("message", ApplicationCommandOptionType.String, "Quote");

    public SlashCommandProperties GetSlashCommand() =>
        SlashCommandBuilder
           .WithName("quote")
           .Build();

    public SlashCommandProperties GetTestSlashCommand() =>
        SlashCommandBuilder
           .WithName("quote-test")
           .Build();
}
