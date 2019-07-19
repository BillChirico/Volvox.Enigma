using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Volvox.Enigma.Domain.User;
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

        public async Task Announce(IEnumerable<Host> hosts, ulong guildId, ulong channelId)
        {
            var channel = _discordClient.GetGuild(guildId)?.GetTextChannel(channelId);

            if (channel == null)
            {
                _logger.LogError("Could not find channel to announce streams!");

                return;
            }

            foreach (var host in hosts)
            {
                var stream = await _twitchApiHelper.GetStream(host.TwitchUsername);

                if (stream == null) continue;

                var game = await _twitchApiHelper.GetStreamGame(stream);

                if (game == null) continue;

                await channel.SendMessageAsync("test");
            }
        }
    }
}