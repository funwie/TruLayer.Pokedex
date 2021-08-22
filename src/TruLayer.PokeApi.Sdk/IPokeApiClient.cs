using System.Threading;
using System.Threading.Tasks;
using TruLayer.PokeApi.Sdk.Models;

namespace TruLayer.PokeApi.Sdk
{
    public interface IPokeApiClient
    {
        Task<PokemonSpeciesResponse> GetPokemonSpecies(string pokemonName, CancellationToken cancellationToken);
    }
}
