using System.Text.Json.Serialization;

namespace Tapanga.Core.Serialization
{
    internal class TapangaMetadata
    {
        public static TapangaMetadata None { get; } = new TapangaMetadata
        {
            ProfileId = null
        };

        [JsonPropertyName("profileId")]
        public Guid? ProfileId { get; set; }
    }
}
