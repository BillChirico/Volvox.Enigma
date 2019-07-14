using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volvox.Enigma.Domain.User;
using Volvox.Enigma.Service.StreamAnnouncer;

namespace Volvox.Enigma.Console
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServiceCollection(new ServiceCollection());

            var streamAnnouncer = serviceProvider.GetRequiredService<StreamAnnouncer>();
        }

        private static ServiceProvider ConfigureServiceCollection(ServiceCollection serviceCollection)
        {
            return serviceCollection
                .Configure<Hosts>(GetSettingsFile("appsettings.json", "HostsConfig"))
                .AddSingleton<IStreamAnnouncer, StreamAnnouncer>()
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