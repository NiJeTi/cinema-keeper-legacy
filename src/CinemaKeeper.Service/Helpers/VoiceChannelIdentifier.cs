using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using CinemaKeeper.Service.Exceptions;

using Discord.WebSocket;

namespace CinemaKeeper.Service.Helpers
{
    internal static class VoiceChannelIdentifier
    {
        public static SocketVoiceChannel Identify(IEnumerable<SocketVoiceChannel> allVoiceChannels,
            VoiceChannelIdType idType, string rawId)
        {
            var voiceChannel = idType switch
            {
                VoiceChannelIdType.Wildcard =>
                    allVoiceChannels.FirstOrDefault(v => Regex.IsMatch(v.Name, rawId, RegexOptions.IgnoreCase)),

                VoiceChannelIdType.Id => allVoiceChannels.FirstOrDefault(v =>
                    v.Id.Equals(ulong.Parse(rawId, CultureInfo.InvariantCulture))),

                _ => throw new InvalidVoiceChannelIdentifierException()
            };

            if (voiceChannel == null)
                throw new VoiceChannelNotFoundException();

            return voiceChannel;
        }

        public static VoiceChannelIdType IdentifyType(string rawId) =>
            Regex.IsMatch(rawId, @"^\w+$")
                ? VoiceChannelIdType.Wildcard
                : Regex.IsMatch(rawId, @"^\d{17,20}$")
                    ? VoiceChannelIdType.Id
                    : VoiceChannelIdType.None;
    }
}
