using System.Threading.Tasks;
using Blizzard.Sdk.Client.Models.Responses;
using RestEase;

namespace Blizzard.Sdk.Client
{
    /// <summary>
    /// A client to get WoW game data
    /// </summary>
    public interface IWowGameDataClient
    {
        /// <summary>
        /// Gets the achievement categories.
        /// </summary>
        /// <param name="region">The region of the data to retrieve.</param>
        /// <param name="namespace">The namespace to use to locate this document.</param>
        /// <param name="locale">The locale to reflect in localized data.</param>
        /// <returns></returns>
        [Get("https://{region}.api.blizzard.com/data/wow/achievement-category/index?namespace={namespace}&locale={locale}")]
        Task<GetAchievementCategoriesResponse> GetAchievementCategories([Path] string region = "us", [Path] string @namespace = "static-us", [Path] string locale = "en_US");
    }
}
