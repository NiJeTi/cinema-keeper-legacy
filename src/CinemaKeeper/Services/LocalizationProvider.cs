using System.Globalization;
using System.Text.RegularExpressions;

using CinemaKeeper.Exceptions;
using CinemaKeeper.Settings;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace CinemaKeeper.Services;

public partial class LocalizationProvider : ILocalizationProvider
{
    private static readonly Regex KeyPattern = GetKeyPatternRegex();

    private readonly ILogger<LocalizationProvider> _logger;
    private readonly IOptionsMonitor<LocalizationSettings> _settings;

    private DateTime _fileModifiedDateTime;
    private JObject? _localizations;

    public LocalizationProvider(
        ILogger<LocalizationProvider> logger,
        IOptionsMonitor<LocalizationSettings> settings)
    {
        _logger = logger;
        _settings = settings;
    }

    private JObject Localizations
    {
        get
        {
            var settings = _settings.CurrentValue;
            var filePath = settings.FilePath;

            if (_localizations is null)
            {
                ReadLocalizationsFile(filePath);

                return _localizations!;
            }

            if (File.GetLastWriteTimeUtc(filePath) <= _fileModifiedDateTime)
                return _localizations;

            ReadLocalizationsFile(filePath);

            return _localizations;
        }
    }

    public string Get(string key) => InternalGet(key);

    public string Get(string key, params object?[] args) =>
        string.Format(CultureInfo.CurrentCulture, InternalGet(key), args);

    private string InternalGet(string key)
    {
        if (!KeyPattern.IsMatch(key))
            throw new ArgumentException($"Key '{key}' has invalid format", nameof(key));

        var indexValues = key.Split('.').Reverse();
        var indexStack = new Stack<string>(indexValues);

        var result = RecursiveRead(Localizations, indexStack).ToString();

        _logger.LogTrace("Found localized string \"{String}\" for key \"{Key}\"", result, key);

        return result;
    }

    private static JToken RecursiveRead(JObject currentObject, Stack<string> indexStack)
    {
        if (!indexStack.TryPop(out var index))
            return currentObject;

        var jToken = currentObject.GetValue(index, StringComparison.OrdinalIgnoreCase)
            ?? throw new LocalizationException($"Localization for key '{index}' was not found");

        if (jToken is JValue)
            return jToken;

        var jObject = jToken as JObject
            ?? throw new LocalizationException($"Localization for key '{index}' is not a {nameof(JObject)}");

        return RecursiveRead(jObject, indexStack);
    }

    private void ReadLocalizationsFile(string filePath)
    {
        _fileModifiedDateTime = File.GetLastWriteTimeUtc(filePath);

        var fileContents = File.ReadAllText(filePath);
        _localizations = JObject.Parse(fileContents);
    }

    [GeneratedRegex(@"^\w+(\.\w+)*$", RegexOptions.Compiled)]
    private static partial Regex GetKeyPatternRegex();
}
