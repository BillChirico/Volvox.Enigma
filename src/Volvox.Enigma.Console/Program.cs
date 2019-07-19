﻿using System.Threading;
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
using Volvox.Enigma.Service.Twitch;

namespace Volvox.Enigma.Console
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServiceCollection(new ServiceCollection());

            // Settings
            var settings = serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
            var hosts = serviceProvider.GetRequiredService<IOptions<Hosts>>().Value;

            // Services
            var streamAnnouncer = serviceProvider.GetRequiredService<IStreamAnnouncer>();

            // Discord
            var discordBot = serviceProvider.GetRequiredService<IDiscordBot>();
            var discordSocketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();

            await discordBot.Connect(settings.DiscordBotToken);

            discordSocketClient.Ready += async () =>
            {
                await streamAnnouncer.Announce(hosts.HostList, settings.DiscordGuildId, settings.DiscordChannelId,
                    settings.DiscordHostRoleId);
            };

            await Task.Delay(Timeout.Infinite);
        }

        private static ServiceProvider ConfigureServiceCollection(ServiceCollection serviceCollection)
        {
            return serviceCollection
                // Settings
                .Configure<Hosts>(GetSettingsFile("appsettings.json", "HostsConfig"))
                .Configure<Settings>(GetSettingsFile("appsettings.json", "Settings"))

                // Services
                .AddSingleton<IStreamAnnouncer, StreamAnnouncer>()

                // Discord
                .AddSingleton<IDiscordBot, DiscordBot>()
                .AddSingleton<DiscordSocketClient>()

                // Twitch Api
                .AddSingleton(provider =>
                    TwitchApiFactory.Create(
                        provider.GetRequiredService<IOptions<Settings>>().Value.TwitchClientId,
                        provider.GetRequiredService<IOptions<Settings>>().Value.TwitchAccessToken))

                // Helpers
                .AddSingleton<ITwitchApiHelper, TwitchApiHelper>()

                // Logging
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