using System.Text.Json.Serialization;

namespace TruLayer.PokeApi.Sdk.Models
{
    public class PokemonSpeciesResponse
    {
        [JsonPropertyName("flavor_text_entries")]
        public FlavorTextEntries[] FlavorTextEntries { get; set; }

        [JsonPropertyName("is_legendary")]
        public bool IsLegendary { get; set; }

        public Habitat Habitat { get; set; }

        public string Name { get; set; }
    }
}
