using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Serilog;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using Volvox.Enigma.Domain.User;
using Volvox.Enigma.Service.Discord;
using Volvox.Enigma.Service.Twitch;
using Game = TwitchLib.Api.Helix.Models.Games.Game;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger _logger;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger logger)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
        }

        public async Task Announce(IEnumerable<Host> hosts, ulong guildId, ulong channelId, ulong roleId)
        {
            var stopWatch = Stopwatch.StartNew();
            _logger.Information("Starting stream announcer");

            try
            {
                var guild = _discordClient.GetGuild(guildId);

                if (guild == null)
                {
                    _logger.Error("Could not find guild!");

                    return;
                }

                var channel = guild.GetTextChannel(channelId);

                if (channel == null)
                {
                    _logger.Error("Could not find channel to announce streams!");

                    return;
                }

                var role = guild.GetRole(roleId);

                if (role == null)
                {
                    _logger.Error("Could not find host role!");

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

                // Delete all previous messages from the bot
                var messages =
                    ( await channel.GetMessagesAsync().FlattenAsync() ).Where(m =>
                        m.Author.Id == _discordClient.CurrentUser.Id);

                await channel.DeleteMessagesAsync(messages);

                await channel.SendMessageAsync(string.Empty,
                    embed: EmbedHelper.GetStreamAnnouncementEmbed("Verified Hosts", "No verified hosts are online!",
                        role.Color, users));
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"Error occured while announcing streams: {exception}");
            }

            stopWatch.Stop();
            _logger.Information($"Finished stream announcer ({stopWatch.ElapsedMilliseconds}ms)");
        }
    }
}