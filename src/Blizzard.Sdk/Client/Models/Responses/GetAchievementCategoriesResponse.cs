using System.Collections.Generic;

namespace Blizzard.Sdk.Client.Models.Responses
{
    /// <summary>
    /// Response for getting achievement categories
    /// </summary>
    public class GetAchievementCategoriesResponse
    {
        /// <summary>
        /// List of categories.
        /// </summary>
        public List<AchievementCategory> Categories { get; set; } = new();
    }
}
