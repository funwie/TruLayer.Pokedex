using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TruLayer.Pokedex.Api.Responses;
using Xunit;

namespace TruLayer.Pokedex.Api.IntegrationTests
{
    public class GetTranslatedPokemonEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private static string GetTranslatedPokemonEndpoint(string pokemonName) => $"/pokemon/{pokemonName}/translated";

        public GetTranslatedPokemonEndpointTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task GetTranslatedPokemonOk()
        {
            var pokemonName = "ditto";
            var getPokemonUrl = GetTranslatedPokemonEndpoint(pokemonName);
            var response = await _httpClient.GetAsync(getPokemonUrl);

            var expectedTranslatedPokemon = response.Content.ReadFromJsonAsync<TranslatedPokemon>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            expectedTranslatedPokemon.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTranslatedPokemonNotFound()
        {
            var notPokemonName = "notPokemonName";
            var getPokemonUrl = GetTranslatedPokemonEndpoint(notPokemonName);
            var response = await _httpClient.GetAsync(getPokemonUrl);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
