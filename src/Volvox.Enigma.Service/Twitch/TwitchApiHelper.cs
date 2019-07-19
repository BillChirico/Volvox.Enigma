using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.Interfaces;

namespace Volvox.Enigma.Service.Twitch
{
    /// <inheritdoc />
    public class TwitchApiHelper : ITwitchApiHelper
    {
        private readonly ITwitchAPI _twitchApi;

        public TwitchApiHelper(ITwitchAPI twitchApi)
        {
            _twitchApi = twitchApi;
        }

        /// <inheritdoc />
        public async Task<User> GetUser(string username)
        {
            var user = ( await _twitchApi.Helix.Users.GetUsersAsync(logins: new List<string> {username}) ).Users
                .FirstOrDefault();

            return user;
        }

        /// <inheritdoc />
        public async Task<Stream> GetStream(User user)
        {
            var stream = ( await _twitchApi.Helix.Streams.GetStreamsAsync(userLogins: new List<string> {user.Login}) )
                .Streams.FirstOrDefault();

            return stream;
        }

        /// <inheritdoc />
        public async Task<Game> GetStreamGame(Stream stream)
        {
            var game = ( await _twitchApi.Helix.Games.GetGamesAsync(new List<string> {stream.GameId}) ).Games
                .FirstOrDefault();

            return game;
        }
    }
}