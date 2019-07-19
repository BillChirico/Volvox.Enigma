using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using Volvox.Enigma.Domain.User;
using Game = TwitchLib.Api.Helix.Models.Games.Game;

namespace Volvox.Enigma.Service.Discord
{
    public static class EmbedHelper
    {
        public static Embed GetStreamAnnouncementEmbed(string title, string offlineMessage, Color color,
            IDictionary<Host, (User, Stream, Game)> users)
        {
            var embedBuilder = new EmbedBuilder
            {
                Color = color,
                Footer = new EmbedFooterBuilder().WithText(
                    $"Last Update: {DateTime.UtcNow.ToShortTimeString()} UTC")
            };

            embedBuilder.WithTitle(
                $"{title} Streaming Now");

            if (users != null && users.Count > 0)
            {
                var streamDescriptions = new List<string>();

                // Add each of the streaming users to the description
                foreach (var (host, (user, stream, game)) in users)
                {
                    if (stream == default || game == default) continue;

                    var description =
                        $"[{user.DisplayName}](https://www.twitch.tv/{user.Login}) | {game.Name} | " 
                        + $"{stream.ViewerCount} Viewers | {host.Region}";

                    // Check if the description is over the Discord limit
                    if (streamDescriptions.Sum(x => x.Length) + description.Length > 2000) break;

                    streamDescriptions.Add(description);
                }


                embedBuilder.WithDescription(string.Join(Environment.NewLine, streamDescriptions));
            }

            else
            {
                embedBuilder.WithDescription(offlineMessage);
            }

            return embedBuilder.Build();
        }
    }
}