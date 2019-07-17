using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace Volvox.Enigma.Service.Twitch
{
    public class TwitchApiFactory
    {
        public static ITwitchAPI Create(string clientId, string accessToken)
        {
            var twitchApi = new TwitchAPI();

            twitchApi.Settings.ClientId = clientId;
            twitchApi.Settings.AccessToken = accessToken;

            return twitchApi;
        }
    }
}