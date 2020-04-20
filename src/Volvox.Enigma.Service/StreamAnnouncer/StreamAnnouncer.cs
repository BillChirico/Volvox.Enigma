using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using Volvox.Enigma.Domain.Settings;
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
        private readonly StreamAnnouncerSettings _settings;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper,
            ILogger logger, IOptions<StreamAnnouncerSettings> settings)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
            _logger = logger;
            _settings = settings.Value;
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
                        !_settings.CreativeStreamTitles.Any(x =>
                            stream.Title.StartsWith(x, StringComparison.InvariantCultureIgnoreCase) ||
                            stream.Title.StartsWith(
                                $"Viewer{x}", StringComparison.InvariantCultureIgnoreCase) ||
                            stream.Title.StartsWith($"Enigma {x}", StringComparison.InvariantCultureIgnoreCase) ||
                            stream.Title.StartsWith($"Enigma's {x}", StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    users.Add(host, ( user, stream, game ));
                }

                // Delete all previous messages from the bot
                var message =
                    ( await channel.GetMessagesAsync().FlattenAsync() ).FirstOrDefault(m =>
                        m.Author.Id == _discordClient.CurrentUser.Id) as RestUserMessage;

                if (message == null)
                {
                    _logger.Information("Sending new announcement message.");

                    await channel.SendMessageAsync(string.Empty,
                        embed: EmbedHelper.GetStreamAnnouncementEmbed("Verified Hosts", "No verified hosts are online!",
                            role.Color, users));
                }

                if (message != null)
                {
                    _logger.Information("Editing announcement message.");

                    await message.ModifyAsync(msg =>
                    {
                        msg.Content = string.Empty;
                        msg.Embed = EmbedHelper.GetStreamAnnouncementEmbed("Verified Hosts",
                            "No verified hosts are online!",
                            role.Color, users);
                    });
                }
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