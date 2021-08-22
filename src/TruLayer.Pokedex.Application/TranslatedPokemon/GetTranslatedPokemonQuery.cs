using MediatR;

namespace TruLayer.Pokedex.Application.TranslatedPokemon
{
    public class GetTranslatedPokemonQuery : IRequest<TranslatedPokemonProjection>
    {
        public string PokemonName { get; set; }
    }
}
