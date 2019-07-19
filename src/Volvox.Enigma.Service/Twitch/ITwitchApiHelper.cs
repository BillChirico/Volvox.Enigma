using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;

namespace Volvox.Enigma.Service.Twitch
{
    /// <summary>
    ///     Helper methods for the Twitch API
    /// </summary>
    public interface ITwitchApiHelper
    {
        /// <summary>
        ///     Get the current stream of the user.
        /// </summary>
        /// <param name="username">User to get the stream.</param>
        /// <returns>Stream of the current user or null if they are not streaming.</returns>
        Task<Stream> GetStream(string username);

        /// <summary>
        ///     Get the game that the user is currently streaming.
        /// </summary>
        /// <param name="stream">Stream to get the game from.</param>
        /// <returns>Game that the user is streaming or null if not found.</returns>
        Task<Game> GetStreamGame(Stream stream);
    }
}