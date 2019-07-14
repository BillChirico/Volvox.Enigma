using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Volvox.Enigma.Domain.User;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly DiscordSocketClient _discordClient;

        public StreamAnnouncer(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public Task Announce(List<Host> hosts)
        {
            throw new NotImplementedException();
        }
    }
}