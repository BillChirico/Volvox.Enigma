using System.Threading.Tasks;

namespace Volvox.Enigma.Service.Discord
{
    public interface IDiscordBot
    {
        Task Connect(string token);
    }
}