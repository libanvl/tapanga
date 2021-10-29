using System.Text.Json.Serialization;

namespace Tapanga.Core.Serialization
{
    internal class TapangaMetadata
    {
        public static TapangaMetadata None { get; } = new TapangaMetadata
        {
            GeneratorId = GeneratorId.None,
            ProfileId = null
        };

        [JsonPropertyName("generatorId")]
        public GeneratorId? GeneratorId { get; set; }

        [JsonPropertyName("profileId")]
        public Guid? ProfileId { get; set; }
    }
}
