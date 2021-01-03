using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Blizzard.Sdk.HttpUtils
{
    /// <summary>
    /// Storage for access token
    /// </summary>
    public interface ICachedAccessTokenProvider
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns></returns>
        TokenResponse? GetAccessToken();

        /// <summary>
        /// Rotates the asynchronous.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> RotateAsync(TokenResponse? accessToken, CancellationToken cancellationToken = default);
    }
}
