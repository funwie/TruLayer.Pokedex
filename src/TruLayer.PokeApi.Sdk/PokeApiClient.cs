using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TruLayer.PokeApi.Sdk.Models;

namespace TruLayer.PokeApi.Sdk
{
    public class PokeApiClient : IPokeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly PokeApiSettings _pokeApiSettings;

        public PokeApiClient(HttpClient httpClient, IOptions<PokeApiSettings> options)
        {
            _httpClient = httpClient;
            _pokeApiSettings = options.Value;
        }

        public async Task<PokemonSpeciesResponse> GetPokemonSpecies(string pokemonName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(pokemonName)) throw new ArgumentNullException(nameof(pokemonName));

            var getPokemonSpeciesUri = $"{_pokeApiSettings.BaseUrl}/pokemon-species/{pokemonName}";
            var pokemonSpecies = await _httpClient.GetFromJsonAsync<PokemonSpeciesResponse>(getPokemonSpeciesUri, cancellationToken);
            return pokemonSpecies;
        }
    }
}