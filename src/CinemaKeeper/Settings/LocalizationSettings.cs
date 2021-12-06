using System;
using System.IO;

namespace CinemaKeeper.Settings;

[Serializable]
public record LocalizationSettings
{
    public string FilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localization.json");
}

