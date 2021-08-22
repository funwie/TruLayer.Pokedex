using System.Text.Json.Serialization;

namespace TruLayer.PokeApi.Sdk.Models
{
    public class FlavorTextEntries
    {
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; set; }
        public Language Language { get; set; }
        public Version Version { get; set; }
    }
}