using Blizzard.Sdk.Options;
using Microsoft.Extensions.Configuration;

namespace Blizzard.Sdk.Tests.Utils
{
    public static class ConfigManager
    {
        static ConfigManager()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddJsonFile("appsettings.test.json", false)
                .AddEnvironmentVariables();

            Configuration = configBuilder.Build();
        }

        private static IConfiguration Configuration { get; }

        public static BlizzardClientOptions ClientOptions => Configuration.GetSection("Client").Get<BlizzardClientOptions>();
    }
}
