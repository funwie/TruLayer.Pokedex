using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using TruLayer.FunTranslations.Sdk;
using TruLayer.FunTranslations.Sdk.Models;
using TruLayer.PokeApi.Sdk;
using TruLayer.PokeApi.Sdk.Models;
using TruLayer.Pokedex.Application.TranslatedPokemon;

namespace TruLayer.Pokedex.Application.Tests
{
    public class GetTranslatedPokemonQueryHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();

        private readonly Mock<IPokeApiClient> _pokeApiClientMock;
        private readonly Mock<IFunTranslationsClient> _funTranslationsClientMock;

        public GetTranslatedPokemonQueryHandlerTests()
        {
            _pokeApiClientMock = new Mock<IPokeApiClient>();
            _funTranslationsClientMock = new Mock<IFunTranslationsClient>();
            var loggerMock = new Mock<ILogger<GetTranslatedPokemonQueryHandler>>();

            _fixture.Inject(_pokeApiClientMock.Object);
            _fixture.Inject(_funTranslationsClientMock.Object);
            _fixture.Inject(loggerMock.Object);
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a translated pokemon
                       AND PokeApi will return success with content
                       AND FunTranslations API will return success with content
                       WHEN the query is handled
                       THEN a translated pokemon is returned")]
        public async Task PokemonIsRequestedAndTranslated()
        {
            var pokemonSpeciesResponse = _fixture.Create<PokemonSpeciesResponse>();
            var translationResponse = _fixture.Create<TranslationResponse>();

            _pokeApiClientMock.Setup(client =>
                    client.GetPokemonSpecies(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pokemonSpeciesResponse);

            _funTranslationsClientMock.Setup(client =>
                    client.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationResponse);

            var expectedTranslatedPokemonProjection = new TranslatedPokemonProjection
            {
                Name = pokemonSpeciesResponse.Name,
                IsLegendary = pokemonSpeciesResponse.IsLegendary,
                Habitat = pokemonSpeciesResponse.Habitat.Name,
                Description = translationResponse.Contents.Text,
                TranslatedDescription = translationResponse.Contents.Translated
            };

            var query = new GetTranslatedPokemonQuery { PokemonName = _fixture.Create<string>() };
            var sut = _fixture.Create<GetTranslatedPokemonQueryHandler>();
            var actualTranslatedPokemonProjection = await sut.Handle(query, CancellationToken.None);

            actualTranslatedPokemonProjection.Should().BeEquivalentTo(expectedTranslatedPokemonProjection);
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a translated pokemon
                       AND PokeApi will return success with content
                       AND pokemon habitat is 'cave'
                       WHEN the query is handled
                       THEN a 'Yoda' translation is requested ")]
        public async Task YodaTranslationIsRequested()
        {
            var habitat = new Habitat { Name = "cave", Url = "http://localhost" };
            var flavorTextEntry = new FlavorTextEntries
            {
                FlavorText = _fixture.Create<string>(),
                Language = new Language { Name = "en", Url = "http://localhost" }
            };

            var pokemonSpeciesResponse = _fixture.Build<PokemonSpeciesResponse>()
                .With(pokemon => pokemon.Habitat, habitat)
                .With(pokemon => pokemon.FlavorTextEntries, new []{ flavorTextEntry })
                .Create();

            var translationResponse = _fixture.Create<TranslationResponse>();

            _pokeApiClientMock.Setup(client =>
                    client.GetPokemonSpecies(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pokemonSpeciesResponse);

            _funTranslationsClientMock.Setup(client =>
                    client.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationResponse)
                .Verifiable();

            var expectedTranslation = "Yoda";

            var query = _fixture.Create<GetTranslatedPokemonQuery>();
            var sut = _fixture.Create<GetTranslatedPokemonQueryHandler>();
            await sut.Handle(query, CancellationToken.None);

            _funTranslationsClientMock.Verify(client =>
                client.Translate(flavorTextEntry.FlavorText, expectedTranslation, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a translated pokemon
                       AND PokeApi will return success with content
                       AND pokemon habitat is not 'cave'
                       WHEN the query is handled
                       THEN a 'Shakespeare' translation is requested ")]
        public async Task ShakespeareTranslationIsRequested()
        {
            var habitat = new Habitat { Name = "other", Url = "http://localhost" };
            var flavorTextEntry = new FlavorTextEntries
            {
                FlavorText = _fixture.Create<string>(),
                Language = new Language { Name = "en", Url = "http://localhost" }
            };

            var pokemonSpeciesResponse = _fixture.Build<PokemonSpeciesResponse>()
                .With(pokemon => pokemon.Habitat, habitat)
                .With(pokemon => pokemon.FlavorTextEntries, new[] { flavorTextEntry })
                .Create();

            var translationResponse = _fixture.Create<TranslationResponse>();

            _pokeApiClientMock.Setup(client =>
                    client.GetPokemonSpecies(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pokemonSpeciesResponse);

            _funTranslationsClientMock.Setup(client =>
                    client.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(translationResponse)
                .Verifiable();

            var expectedTranslation = "Shakespeare";

            var query = _fixture.Create<GetTranslatedPokemonQuery>();
            var sut = _fixture.Create<GetTranslatedPokemonQueryHandler>();
            await sut.Handle(query, CancellationToken.None);

            _funTranslationsClientMock.Verify(client =>
                client.Translate(flavorTextEntry.FlavorText, expectedTranslation, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}