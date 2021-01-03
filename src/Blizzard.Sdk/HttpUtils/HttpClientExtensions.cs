using System.Threading;
using System.Threading.Tasks;
using Blizzard.Sdk.Options;
using IdentityModel.Client;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    /// <summary>
    /// Extension methods for http client
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Requests the client credentials token asynchronous.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="authOptions">The authentication options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestClientCredentialsTokenAsync(this HttpClient httpClient, BlizzardClientOptions authOptions, CancellationToken cancellationToken = default)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (authOptions == null) throw new ArgumentNullException(nameof(authOptions));

            return await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = "https://us.battle.net/oauth/token",
                    ClientId = authOptions.ClientId,
                    ClientSecret = authOptions.Secret,
                },
                cancellationToken);
        }
    }
}
