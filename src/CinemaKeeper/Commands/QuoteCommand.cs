using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
[RequireBotPermission(GuildPermission.SendMessages | GuildPermission.EmbedLinks)]
public class QuoteCommand : InteractionModuleBase, ISlashCommandBuilder
{
    private const string Command = "quote";

    private readonly ILogger _logger;
    private readonly IAsyncDatabaseReader _databaseReader;
    private readonly IAsyncDatabaseWriter _databaseWriter;
    private readonly ILocalizationProvider _localization;

    private readonly Dictionary<ulong, IUser> _userCache = new();

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
           .AddOption("text", ApplicationCommandOptionType.String, messageOption)
           .Build();
    }

    [SlashCommand(Command, "")]
    public async Task Execute(IUser author, string? text = null)
    {
        await DeferAsync();

        if (text is null)
            await ListQuotes(author, Context.Guild.Id);
        else
            await AddQuote(author, text);
    }

    private async Task ListQuotes(IUser author, ulong guildId)
    {
        var quotes = await _databaseReader.GetUserQuotesAsync(author.Id, guildId);

        if (!quotes.Any())
        {
            var response = _localization.Get("commands.quote.noQuotesForUser", author.Mention);
            await FollowupAsync(response);

            _logger.Debug("No quotes for \"{User}\"", author.GetFullUsername());
        }
        else
        {
            var mainEmbed = BuildMainEmbed(author);
            var quotesEmbeds = await BuildQuotesEmbeds(quotes);

            await FollowupAsync(embeds: new[] { mainEmbed });

            const int maxEmbedsPerMessage = 10;
            var partitions = Partitioner.Create(0, quotes.Count, maxEmbedsPerMessage).GetDynamicPartitions();

            foreach (var (from, to) in partitions)
            {
                var partitionEmbeds = quotesEmbeds[from..to];
                await Context.Channel.SendMessageAsync(embeds: partitionEmbeds);
            }

            _logger.Debug("Printed {QuoteCount} quote(s) of {User}", quotes.Count, author.GetFullUsername());
        }
    }

    private async Task AddQuote(IUser author, string text)
    {
        var addedBy = Context.User;
        var createdOn = Context.Guild.Id;
        var timestamp = DateTimeOffset.UtcNow;

        var quote = new Quote(author.Id, text, timestamp, addedBy.Id, createdOn);
        await _databaseWriter.AddQuoteAsync(quote);

        await FollowupAsync(embed: BuildNewQuoteEmbed(text, addedBy, author, timestamp));

        _logger.Debug("Added quote of \"{User}\"", author.GetFullUsername());
    }

    private Embed BuildNewQuoteEmbed(string text, IUser addedBy, IUser author, DateTimeOffset timestamp)
    {
        var title = _localization.Get("commands.quote.newQuote");

        return new EmbedBuilder()
           .WithTitle(title)
           .WithDescription(text)
           .WithAuthor(builder => builder
               .WithName(addedBy.Username)
               .WithIconUrl(addedBy.GetAvatarUrl()))
           .WithFooter(builder => builder
               .WithText(author.Username)
               .WithIconUrl(author.GetAvatarUrl()))
           .WithTimestamp(timestamp)
           .WithColor(GetRandomColor())
           .Build();
    }

    private Embed BuildMainEmbed(IUser author)
    {
        var title = _localization.Get("commands.quote.title");
        var description = _localization.Get("commands.quote.description", author.Mention);

        return new EmbedBuilder()
           .WithTitle(title)
           .WithDescription(description)
           .WithThumbnailUrl(author.GetAvatarUrl())
           .WithColor(GetRandomColor())
           .Build();
    }

    private async Task<Embed[]> BuildQuotesEmbeds(ReadOnlyCollection<Quote> quotes)
    {
        var embeds = new Embed[quotes.Count];

        for (var i = 0; i < quotes.Count; i++)
        {
            var quote = quotes[i];

            var userFoundInCache = _userCache.ContainsKey(quote.CreatedBy);

            var addedBy = userFoundInCache
                ? _userCache[quote.CreatedBy]
                : await Context.Client.GetUserAsync(quote.CreatedBy);

            _userCache[quote.CreatedBy] = addedBy;

            var embedBuilder = new EmbedBuilder()
               .WithTitle(quote.Text)
               .WithFooter(builder => builder
                   .WithText(addedBy.Username)
                   .WithIconUrl(addedBy.GetAvatarUrl()))
               .WithTimestamp(quote.CreatedAt)
               .WithColor(GetRandomColor());

            embeds[i] = embedBuilder.Build();
        }

        return embeds;
    }

    private static Color GetRandomColor()
    {
        var colorBytes = new byte[3];
        new Random().NextBytes(colorBytes);

        return new Color(colorBytes[0], colorBytes[1], colorBytes[2]);
    }

    // TODO: Use when there will be valid interfaces for components
    private static MessageComponent BuildPaginationControls() =>
        new ComponentBuilder()
           .WithButton(customId: "previous", emote: new Emoji("◀"), style: ButtonStyle.Secondary)
           .WithButton(customId: "next", emote: new Emoji("▶"), style: ButtonStyle.Secondary)
           .Build();
}
