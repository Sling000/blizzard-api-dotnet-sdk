namespace Blizzard.Sdk.Options
{
    /// <summary>
    /// Options to authorize with blizzard api. Get your client id and secret here: https://develop.battle.net/access/clients
    /// </summary>
    public class BlizzardClientOptions
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        public string? Secret { get; set; }
    }
}
