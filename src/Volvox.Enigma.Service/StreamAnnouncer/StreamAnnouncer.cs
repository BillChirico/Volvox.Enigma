using Microsoft.Extensions.Options;
using Volvox.Enigma.Domain.User;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public class StreamAnnouncer : IStreamAnnouncer
    {
        private readonly Hosts _hosts;

        public StreamAnnouncer(IOptions<Hosts> hosts)
        {
            _hosts = hosts.Value;
        }
    }
}