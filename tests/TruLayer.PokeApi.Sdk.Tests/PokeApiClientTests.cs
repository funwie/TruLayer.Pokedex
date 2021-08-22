using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TruLayer.PokeApi.Sdk.Models;

namespace TruLayer.PokeApi.Sdk.Tests
{
    public class PokeApiClientTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Test]
        [Description(@"GIVEN a pokemon name
                       AND PokeApi request will return success with content
                       WHEN Translate is called
                       THEN expected translation response is returned")]
        public async Task PokemonSpeciesRequestIsSuccessful()
        {
            var responseFromApi = _fixture.Create<PokemonSpeciesResponse>();
            var httpClientMock = HttpClientMock(HttpStatusCode.OK, responseFromApi);
            var pokeApiSettings = new PokeApiSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(pokeApiSettings);
            var sut = new PokeApiClient(httpClientMock, options);

            var expectedPokemonSpeciesResponse = new PokemonSpeciesResponse
            {
                IsLegendary = responseFromApi.IsLegendary,
                Name = responseFromApi.Name,
                Habitat = new Habitat
                {
                    Name = responseFromApi.Habitat.Name,
                    Url = responseFromApi.Habitat.Url
                },
                FlavorTextEntries = responseFromApi.FlavorTextEntries
            };

            var pokemonName = _fixture.Create<string>();
            var actualPokemonSpeciesResponse = await sut.GetPokemonSpecies(pokemonName, CancellationToken.None);

            actualPokemonSpeciesResponse.Should().BeEquivalentTo(expectedPokemonSpeciesResponse);
        }

        [TestCase(null)]
        [TestCase("")]
        [Description(@"GIVEN null or empty pokemonName
                       WHEN GetPokemonSpecies is called
                       THEN ArgumentNullException is thrown")]
        public async Task InvalidInput(string pokemonName)
        {
            var httpClientMock = HttpClientMock(HttpStatusCode.OK, null);
            var pokeApiSettings = new PokeApiSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(pokeApiSettings);
            
            var sut = new PokeApiClient(httpClientMock, options);
            Func<Task> act = async () => await sut.GetPokemonSpecies(pokemonName, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        [Description(@"GIVEN a text and a translation
                       AND the request to Fun Translation API will throw an exception
                       WHEN Translate is called
                       THEN the exception is rethrown")]
        public async Task GetPokemonSpeciesRequestThrowsException()
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            var httpClientMock = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };

            var pokeApiSettings = new PokeApiSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(pokeApiSettings);

            var pokemonName = _fixture.Create<string>();
            var sut = new PokeApiClient(httpClientMock, options);
            Func<Task> act = async () => await sut.GetPokemonSpecies(pokemonName, CancellationToken.None);

            await act.Should().ThrowAsync<HttpRequestException>();
        }

        private static HttpClient HttpClientMock(HttpStatusCode statusCode, PokemonSpeciesResponse responseContent = null)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = JsonContent.Create(responseContent)
                })
                .Verifiable();

            return new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        }
    }
}