using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CinemaKeeper.Extensions;
using CinemaKeeper.Services;
using CinemaKeeper.Storage;
using CinemaKeeper.Storage.Models;

using Discord;
using Discord.Interactions;

using Serilog;

namespace CinemaKeeper.Commands;

[RequireContext(ContextType.Guild)]
[RequireBotPermission(GuildPermission.SendMessages)]
public class QuoteCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "quote";

    private readonly ILogger _logger;
    private readonly IAsyncDatabaseReader _databaseReader;
    private readonly IAsyncDatabaseWriter _databaseWriter;
    private readonly ILocalizationProvider _localization;

    public QuoteCommand(
        ILogger logger,
        IAsyncDatabaseReader databaseReader,
        IAsyncDatabaseWriter databaseWriter,
        ILocalizationProvider localization)
    {
        _logger = logger.ForContext<QuoteCommand>();
        _databaseReader = databaseReader;
        _databaseWriter = databaseWriter;
        _localization = localization;
    }

    public SlashCommandProperties Build()
    {
        var command = _localization.Get("commands.quote.definition.command");
        var authorOption = _localization.Get("commands.quote.definition.authorOption");
        var messageOption = _localization.Get("commands.quote.definition.messageOption");

        return new SlashCommandBuilder()
           .WithName(Command)
           .WithDescription(command)
           .AddOption("author", ApplicationCommandOptionType.User, authorOption, true)
           .AddOption("message", ApplicationCommandOptionType.String, messageOption)
           .Build();
    }

    [SlashCommand(Command, "")]
    public async Task Execute(IUser author, string? message = null)
    {
        await DeferAsync();

        if (message is null)
        {
            var quotes = await _databaseReader.GetUserQuotesAsync(author.Id);

            if (!quotes.Any())
            {
                var response = _localization.Get("commands.quote.noQuotesForUser", author.Mention);
                await FollowupAsync(response);

                _logger.Debug("No quotes for \"{User}\"", author.GetFullUsername());
            }
            else
            {
                var embeds = await BuildQuotesEmbeds(quotes, author);
                await FollowupAsync(embeds: embeds);

                _logger.Debug("Printed {QuoteCount} quote(s) of {User}", quotes.Count, author.GetFullUsername());
            }
        }
        else
        {
            var quote = new Quote(author.Id, message, DateTimeOffset.UtcNow, Context.User.Id);
            await _databaseWriter.AddQuoteAsync(quote);

            var response = _localization.Get("commands.quote.quoteAdded", author.Mention);
            await FollowupAsync(response);

            _logger.Debug("Added quote of \"{User}\"", author.GetFullUsername());
        }
    }

    private async Task<Embed[]> BuildQuotesEmbeds(ReadOnlyCollection<Quote> quotes, IUser author)
    {
        var embeds = new Embed[1 + quotes.Count];

        var title = _localization.Get("commands.quote.title");
        var description = _localization.Get("commands.quote.description", author.Mention);

        var mainEmbed = new EmbedBuilder()
           .WithTitle(title)
           .WithDescription(description)
           .WithThumbnailUrl(author.GetAvatarUrl())
           .WithColor(GetRandomColor());

        embeds[0] = mainEmbed.Build();

        for (var i = 0; i < quotes.Count; i++)
        {
            var quote = quotes[i];

            var addedBy = await Context.Client.GetUserAsync(quote.CreatedBy);

            var embedBuilder = new EmbedBuilder()
               .WithTitle(quote.Text)
               .WithFooter(builder => builder
                   .WithText(addedBy.Username)
                   .WithIconUrl(addedBy.GetAvatarUrl()))
               .WithTimestamp(quote.CreatedAt)
               .WithColor(GetRandomColor());

            embeds[i + 1] = embedBuilder.Build();
        }

        return embeds;
    }

    private static Color GetRandomColor()
    {
        var colorBytes = new byte[3];
        new Random().NextBytes(colorBytes);

        return new Color(colorBytes[0], colorBytes[1], colorBytes[2]);
    }
}
