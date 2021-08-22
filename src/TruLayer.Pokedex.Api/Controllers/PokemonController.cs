using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TruLayer.Pokedex.Api.Responses;
using TruLayer.Pokedex.Application.Pokemon;
using TruLayer.Pokedex.Application.TranslatedPokemon;

namespace TruLayer.Pokedex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PokemonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PokemonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves Pokemon information.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon</param>
        /// <returns>Pokemon</returns>
        /// <response code="200">Returns the pokemon</response>
        /// <response code="404">Pokemon with name not found</response>
        [HttpGet("{pokemonName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pokemon>> GetPokemon(string pokemonName, CancellationToken cancellationToken)
        {
            var query = new GetPokemonQuery { PokemonName = pokemonName };
            var queryResult = await _mediator.Send(query, cancellationToken);
            if (queryResult is null) return NotFound();

            return new Pokemon
            {
                Name = queryResult.Name,
                Description = queryResult.Description,
                IsLegendary = queryResult.IsLegendary,
                Habitat = queryResult.Habitat
            };
        }

        /// <summary>
        /// Retrieves Pokemon information with translated description.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon</param>
        /// <returns>TranslatedPokemon</returns>
        /// <response code="200">Returns the translated pokemon</response>
        /// <response code="404">Pokemon with name not found</response>
        [HttpGet("{pokemonName}/Translated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TranslatedPokemon>> GetTranslatedPokemon(string pokemonName, CancellationToken cancellationToken)
        {
            var query = new GetTranslatedPokemonQuery { PokemonName = pokemonName };
            var queryResult = await _mediator.Send(query, cancellationToken);

            if (queryResult is null) return NotFound();

            return new TranslatedPokemon
            {
                Name = queryResult.Name,
                Description = queryResult.Description,
                IsLegendary = queryResult.IsLegendary,
                Habitat = queryResult.Habitat,
                TranslatedDescription = queryResult.TranslatedDescription
            };
        }
    }
}
