using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using CinemaKeeper.Exceptions;

using Discord.WebSocket;

namespace CinemaKeeper.Commands.Helpers;

internal static class VoiceChannelIdentifier
{
    public static SocketVoiceChannel Identify(
        IEnumerable<SocketVoiceChannel> allVoiceChannels,
        VoiceChannelIdType idType,
        string rawId)
    {
        var voiceChannel = idType switch
        {
            VoiceChannelIdType.Wildcard =>
                allVoiceChannels.FirstOrDefault(v => Regex.IsMatch(v.Name, rawId, RegexOptions.IgnoreCase)),
            VoiceChannelIdType.Id =>
                allVoiceChannels.FirstOrDefault(v => v.Id.Equals(ulong.Parse(rawId, CultureInfo.InvariantCulture))),
            _ => throw new InvalidVoiceChannelIdentifierException()
        };

        return voiceChannel ?? throw new VoiceChannelNotFoundException();
    }

    public static VoiceChannelIdType IdentifyType(string rawId) =>
        Regex.IsMatch(rawId, @"^\w+$")
            ? VoiceChannelIdType.Wildcard
            : Regex.IsMatch(rawId, @"^\d{17,20}$")
                ? VoiceChannelIdType.Id
                : VoiceChannelIdType.None;
}
