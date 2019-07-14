using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volvox.Enigma.Domain.Settings;
using Volvox.Enigma.Domain.User;
using Volvox.Enigma.Service.Discord;
using Volvox.Enigma.Service.StreamAnnouncer;

namespace Volvox.Enigma.Console
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServiceCollection(new ServiceCollection());

            var settings = serviceProvider.GetRequiredService<IOptions<Settings>>().Value;

            var discordBot = serviceProvider.GetRequiredService<IDiscordBot>();

            await discordBot.Connect(settings.DiscordBotToken);

            await Task.Delay(Timeout.Infinite);
        }

        private static ServiceProvider ConfigureServiceCollection(ServiceCollection serviceCollection)
        {
            return serviceCollection
                .Configure<Hosts>(GetSettingsFile("appsettings.json", "HostsConfig"))
                .Configure<Settings>(GetSettingsFile("appsettings.json", "Settings"))
                .AddSingleton<IStreamAnnouncer, StreamAnnouncer>()
                .AddSingleton<IDiscordBot, DiscordBot>()
                .AddSingleton<DiscordSocketClient>()
                .AddLogging(configure => configure.AddConsole())
                .BuildServiceProvider();
        }

        private static IConfigurationSection GetSettingsFile(string file, string section)
        {
            var builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(file, false, true);

            var configuration = builder.Build();

            return configuration.GetSection(section);
        }
    }
}