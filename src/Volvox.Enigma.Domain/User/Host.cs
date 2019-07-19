namespace Volvox.Enigma.Domain.User
{
    public class Host
    {
        public ulong DiscordId { get; set; }

        public string TwitchUsername { get; set; }

        public Region Region { get; set; }
    }

    public enum Region
    {
        NAE,
        EU
    }
}