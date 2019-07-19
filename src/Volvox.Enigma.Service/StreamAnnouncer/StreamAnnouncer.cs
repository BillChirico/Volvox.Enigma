using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using Volvox.Enigma.Domain.User;
using Volvox.Enigma.Service.Discord;
using Volvox.Enigma.Service.Twitch;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<StreamAnnouncer> _logger;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger<StreamAnnouncer> logger)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
        }

        public async Task Announce(IEnumerable<Host> hosts, ulong guildId, ulong channelId, ulong roleId)
        {
            var stopWatch = Stopwatch.StartNew();
            _logger.LogInformation("Starting stream announcer");

            var guild = _discordClient.GetGuild(guildId);

            if (guild == null)
            {
                _logger.LogError("Could not find guild!");

                return;
            }

            var channel = guild.GetTextChannel(channelId);

            if (channel == null)
            {
                _logger.LogError("Could not find channel to announce streams!");

                return;
            }

            var role = guild.GetRole(roleId);

            if (role == null)
            {
                _logger.LogError("Could not find host role!");

                return;
            }

            var users = new Dictionary<Host, (User, Stream, Game)>();

            foreach (var host in hosts)
            {
                var user = await _twitchApiHelper.GetUser(host.TwitchUsername);

                if (user == null) continue;

                var stream = await _twitchApiHelper.GetStream(user);

                if (stream == null) continue;

                var game = await _twitchApiHelper.GetStreamGame(stream);

                if (game == null) continue;

                // Only announce Fortnite and Zone Wars
                if (game.Name != "Fortnite" ||
                    !stream.Title.StartsWith("Zone Wars", StringComparison.InvariantCultureIgnoreCase)) continue;

                users.Add(host, ( user, stream, game ));
            }

            await channel.SendMessageAsync(string.Empty,
                embed: EmbedHelper.GetStreamAnnouncementEmbed("Verified Hosts", "No verified hosts are online!",
                    role.Color, users));

            stopWatch.Stop();

            _logger.LogInformation("Finished stream announcer ({ElapsedMilliseconds}ms)", stopWatch.ElapsedMilliseconds);
        }
    }
}