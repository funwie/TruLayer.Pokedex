using MediatR;

namespace TruLayer.Pokedex.Application.Pokemon
{
    public class GetPokemonQuery : IRequest<PokemonProjection>
    {
        public string PokemonName { get; set; }
    }
}
