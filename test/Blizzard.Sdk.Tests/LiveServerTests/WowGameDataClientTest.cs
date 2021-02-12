using System.Net.Http;
using System.Threading.Tasks;
using Blizzard.Sdk.Client;
using Blizzard.Sdk.HttpUtils;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using RestEase.HttpClientFactory;
using Xunit;

namespace Blizzard.Sdk.Tests.LiveServerTests
{
    public class WowGameDataClientTest
    {
        [Fact]
        public async Task Test()
        {
            var services = new ServiceCollection();

            services.AddHttpClient<ICachedAccessTokenProvider, CachedAccessTokenProvider>();
            services.AddTransient<CachedTokenDelegatingHandler>();
            
            services
                .AddRestEaseClient<IWowGameDataClient>("https://api.blizzard.com")
                .AddHttpMessageHandler<CachedTokenDelegatingHandler>();

            var sp = services.BuildServiceProvider();
            
            var client = sp.GetRequiredService<IWowGameDataClient>();
            
            var response = await client.GetAchievementCategories();
        }
    }
}
