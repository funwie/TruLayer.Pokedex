using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TruLayer.PokeApi.Sdk;
using TruLayer.PokeApi.Sdk.Models;

namespace TruLayer.Pokedex.Application.Pokemon
{
    public class GetPokemonQueryHandler : IRequestHandler<GetPokemonQuery, PokemonProjection>
    {
        private readonly IPokeApiClient _pokeApiClient;
        private readonly ILogger<GetPokemonQueryHandler> _logger;

        public GetPokemonQueryHandler(IPokeApiClient pokeApiClient, ILogger<GetPokemonQueryHandler> logger)
        {
            _pokeApiClient = pokeApiClient;
            _logger = logger;
        }

        public async Task<PokemonProjection> Handle(GetPokemonQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request?.PokemonName)) throw new ArgumentNullException(nameof(request));

            try
            {
                var pokemonSpecies = await _pokeApiClient.GetPokemonSpecies(request.PokemonName, cancellationToken);
                return Map(pokemonSpecies);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get pokemon");
            }

            return null;
        }

        private PokemonProjection Map(PokemonSpeciesResponse pokemonSpeciesResponse)
        {
            if (pokemonSpeciesResponse is null) return null;

            return new PokemonProjection
            {
                Name = pokemonSpeciesResponse.Name,
                Habitat = pokemonSpeciesResponse.Habitat?.Name,
                IsLegendary = pokemonSpeciesResponse.IsLegendary,
                Description = ParseEnglishDescription(pokemonSpeciesResponse.FlavorTextEntries)
            };
        }

        private static string ParseEnglishDescription(FlavorTextEntries[] flavorTextEntries)
        {
            var englishDescription = flavorTextEntries?.FirstOrDefault(text => string.Equals(text.Language.Name, "en"))?.FlavorText ?? "";
            var newLinesRemovedDescription = Regex.Replace(englishDescription, @"\t|\n|\r|\f", " ");
            return newLinesRemovedDescription;
        }
    }
}
