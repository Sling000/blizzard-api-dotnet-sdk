using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blizzard.Sdk.Options;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace Blizzard.Sdk.HttpUtils
{
    /// <summary>
    /// Access token cache
    /// </summary>
    /// <seealso cref="ICachedAccessTokenProvider" />
    /// <seealso cref="System.IDisposable" />
    public class CachedAccessTokenProvider : ICachedAccessTokenProvider, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The cached map of hosts/token responses
        /// </summary>
        private readonly TokenInfo _tokenInfo;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAccessTokenProvider"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="clientOptions">The authentication options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// httpClient
        /// or
        /// authOptions
        /// </exception>
        public CachedAccessTokenProvider(HttpClient httpClient, IOptions<BlizzardClientOptions> clientOptions)
        {
            this._httpClient = httpClient;

            if (clientOptions.Value == null) throw new ArgumentNullException(nameof(clientOptions));
            this._tokenInfo = new TokenInfo(clientOptions.Value);
        }

        /// <inheritdoc />
        public TokenResponse? GetAccessToken()
        {
            return this._tokenInfo.Token;
        }

        /// <inheritdoc />
        public async Task<bool> RotateAsync(TokenResponse? accessToken, CancellationToken cancellationToken = default)
        {
            if (this._tokenInfo.Token != accessToken)
            {
                return true;
            }

            var semaphore = this._tokenInfo.Semaphore;

            if (!await semaphore.WaitAsync(this._timeout, cancellationToken))
            {
                return false;
            }

            try
            {
                if (this._tokenInfo.Token != accessToken)
                {
                    return true;
                }

                var response = await this._httpClient.RequestClientCredentialsTokenAsync(this._tokenInfo.AuthOptions, cancellationToken);

                if (response.IsError)
                {
                    return false;
                }

                this._tokenInfo.Token = response;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._tokenInfo.Dispose();
        }

        /// <summary>
        /// Internal store for pair of semaphore and access token
        /// </summary>
        private class TokenInfo : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TokenInfo"/> class.
            /// </summary>
            /// <param name="authOptions">The authentication options.</param>
            public TokenInfo(BlizzardClientOptions authOptions)
            {
                this.AuthOptions = authOptions;
            }

            /// <summary>
            /// Gets the authentication options.
            /// </summary>
            /// <value>
            /// The authentication options.
            /// </value>
            public BlizzardClientOptions AuthOptions { get; }

            /// <summary>
            /// Gets the semaphore.
            /// </summary>
            /// <value>
            /// The semaphore.
            /// </value>
            public SemaphoreSlim Semaphore { get; } = new(1, 1);

            /// <summary>
            /// Gets or sets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            public TokenResponse? Token { get; set; }

            /// <inheritdoc />
            public void Dispose()
            {
                this.Semaphore.Dispose();
            }
        }
    }
}
