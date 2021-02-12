using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Blizzard.Sdk.HttpUtils;
using Blizzard.Sdk.Options;
using Microsoft.Extensions.Options;
using Xunit;

namespace Blizzard.Sdk.Tests.Utils
{
    public class PocTest
    {
        [Fact]
        public async Task Test()
        {
            var clientOptions = ConfigManager.ClientOptions;

            var cachedTokenProvider = new CachedAccessTokenProvider(new HttpClient(), new OptionsWrapper<BlizzardClientOptions>(clientOptions));

            var tokenHandler = new CachedTokenDelegatingHandler(cachedTokenProvider, new HttpClientHandler());

            var client = new HttpClient(tokenHandler)
            {
                BaseAddress = new Uri("https://us.api.blizzard.com"),
            };

            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "data/wow/achievement-category/index?namespace=static-us&locale=en_US"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
