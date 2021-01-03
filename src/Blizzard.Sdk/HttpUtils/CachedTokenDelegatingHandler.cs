using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Blizzard.Sdk.HttpUtils
{
    /// <summary>
    /// Delegating handler to get and cache token
    /// </summary>
    public class CachedTokenDelegatingHandler : DelegatingHandler
    {
        private readonly ICachedAccessTokenProvider _cachedAccessTokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedTokenDelegatingHandler"/> class.
        /// </summary>
        /// <param name="cachedAccessTokenProvider">The cached access token provider.</param>
        /// <exception cref="ArgumentNullException">cachedAccessTokenProvider</exception>
        public CachedTokenDelegatingHandler(ICachedAccessTokenProvider cachedAccessTokenProvider)
        {
            this._cachedAccessTokenProvider = cachedAccessTokenProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedTokenDelegatingHandler"/> class.
        /// </summary>
        /// <param name="cachedAccessTokenProvider">The authentication cache.</param>
        /// <param name="innerHandler">The inner handler.</param>
        /// <exception cref="ArgumentNullException">authenticationCache</exception>
        public CachedTokenDelegatingHandler(ICachedAccessTokenProvider cachedAccessTokenProvider, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            this._cachedAccessTokenProvider = cachedAccessTokenProvider;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = this._cachedAccessTokenProvider.GetAccessToken();
            if (accessToken == null)
            {
                if (await this._cachedAccessTokenProvider.RotateAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    accessToken = this._cachedAccessTokenProvider.GetAccessToken();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
                }
            }

            if (accessToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (await this._cachedAccessTokenProvider.RotateAsync(accessToken, cancellationToken).ConfigureAwait(false))
            {
                accessToken = this._cachedAccessTokenProvider.GetAccessToken();
            }
            else
            {
                return response;
            }

            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            if (accessToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
