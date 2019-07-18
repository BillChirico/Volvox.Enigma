using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Volvox.Enigma.Domain.User;
using Volvox.Enigma.Service.Twitch;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ITwitchApiHelper _twitchApiHelper;

        public StreamAnnouncer(DiscordSocketClient discordClient, ITwitchApiHelper twitchApiHelper)
        {
            _discordClient = discordClient;
            _twitchApiHelper = twitchApiHelper;
        }

        public async Task Announce(IEnumerable<Host> hosts, ulong guildId, ulong channelId)
        {
            foreach (var host in hosts)
            {
                var game = await _twitchApiHelper.GetStreamGame(host.TwitchUsername);

                if (game == null) continue;

                var channel = _discordClient.GetGuild(guildId)?.GetTextChannel(channelId);

                if (channel == null) continue;

                await channel.SendMessageAsync("test");
            }
        }
    }
}