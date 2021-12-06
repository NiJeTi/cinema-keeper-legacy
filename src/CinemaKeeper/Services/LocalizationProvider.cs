using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CinemaKeeper.Exceptions;
using CinemaKeeper.Settings;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

namespace CinemaKeeper.Services;

public class LocalizationProvider : ILocalizationProvider
{
    private static readonly Regex KeyPattern = new(@"^[a-zA-Z]\w*(\.[a-zA-Z]\w*)*$", RegexOptions.Compiled);

    private readonly IOptionsMonitor<LocalizationSettings> _settings;

    private DateTime _fileModifiedDateTime;
    private JObject? _localizations;

    public LocalizationProvider(IOptionsMonitor<LocalizationSettings> settings)
    {
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

    public string Get(string key) =>
        InternalGet(key);

    public string Get(string key, params object?[] args) =>
        string.Format(CultureInfo.CurrentCulture, InternalGet(key), args);

    private string InternalGet(string key)
    {
        if (!KeyPattern.IsMatch(key))
            throw new ArgumentException($"Key '{key}' has invalid format", nameof(key));

        var indexValues = key.Split('.').Reverse();
        var indexStack = new Stack<string>(indexValues);

        return RecursiveRead(Localizations, indexStack).ToString();
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
            ?? throw new LocalizationException($"Localization for key '{index}' is not a JObject");

        return RecursiveRead(jObject, indexStack);
    }

    private void ReadLocalizationsFile(string filePath)
    {
        _fileModifiedDateTime = File.GetLastWriteTimeUtc(filePath);

        var fileContents = File.ReadAllText(filePath);
        _localizations = JObject.Parse(fileContents);
    }
}
