using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TruLayer.FunTranslations.Sdk;
using TruLayer.FunTranslations.Sdk.Models;
using TruLayer.PokeApi.Sdk;
using TruLayer.PokeApi.Sdk.Models;

namespace TruLayer.Pokedex.Application.TranslatedPokemon
{
    public class GetTranslatedPokemonQueryHandler : IRequestHandler<GetTranslatedPokemonQuery, TranslatedPokemonProjection>
    {
        private readonly IPokeApiClient _pokeApiClient;
        private readonly IFunTranslationsClient _funTranslationsClient;
        private readonly ILogger<GetTranslatedPokemonQueryHandler> _logger;

        public GetTranslatedPokemonQueryHandler(IPokeApiClient pokeApiClient, IFunTranslationsClient funTranslationsClient, ILogger<GetTranslatedPokemonQueryHandler> logger)
        {
            _pokeApiClient = pokeApiClient;
            _funTranslationsClient = funTranslationsClient;
            _logger = logger;
        }

        public async Task<TranslatedPokemonProjection> Handle(GetTranslatedPokemonQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request?.PokemonName)) throw new ArgumentNullException(nameof(request));

            var pokemon = await GetPostPokemonSpecies(request.PokemonName, cancellationToken);
            if (pokemon is null) return null;

            var description = ParseEnglishDescription(pokemon.FlavorTextEntries);
            var habitat = pokemon.Habitat?.Name;

            var translation = await TranslateDescription(description, habitat, cancellationToken);

            return translation is null ? null : Map(pokemon, translation);
        }

        private async Task<PokemonSpeciesResponse> GetPostPokemonSpecies(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                return await _pokeApiClient.GetPokemonSpecies(pokemonName, cancellationToken);
                
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get pokemon");
            }

            return null;
        }

        private async Task<TranslationResponse> TranslateDescription(string description, string habitat, CancellationToken cancellationToken)
        {
            var translation = string.Equals(habitat, "cave", StringComparison.InvariantCultureIgnoreCase)
                ? "Yoda"
                : "Shakespeare";
            try
            {
                return await _funTranslationsClient.Translate(description, translation, cancellationToken);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to translate pokemon description");
            }

            return null;
        }

        private TranslatedPokemonProjection Map(PokemonSpeciesResponse pokemonSpeciesResponse, TranslationResponse translationResponse)
        {
            if (pokemonSpeciesResponse is null) return null;
            return new TranslatedPokemonProjection
            {
                Name = pokemonSpeciesResponse.Name,
                Habitat = pokemonSpeciesResponse.Habitat?.Name,
                IsLegendary = pokemonSpeciesResponse.IsLegendary,
                TranslatedDescription = translationResponse.Contents?.Translated,
                Description = translationResponse.Contents?.Text
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
