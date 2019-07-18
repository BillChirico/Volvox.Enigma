using System.Collections.Generic;
using System.Threading.Tasks;
using Volvox.Enigma.Domain.User;

namespace Volvox.Enigma.Service.StreamAnnouncer
{
    public interface IStreamAnnouncer
    {
        Task Announce(IEnumerable<Host> hosts, ulong guildId, ulong channelId);
    }
}