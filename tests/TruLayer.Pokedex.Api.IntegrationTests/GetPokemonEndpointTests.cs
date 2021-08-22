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
    public class GetPokemonEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private static string GetPokemonEndpoint(string pokemonName) => $"/pokemon/{pokemonName}";

        public GetPokemonEndpointTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task GetPokemonOk()
        {
            var pokemonName = "mewtwo";
            var getPokemonUrl = GetPokemonEndpoint(pokemonName);
            var response = await _httpClient.GetAsync(getPokemonUrl);

            var expectedPokemon = response.Content.ReadFromJsonAsync<Pokemon>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            expectedPokemon.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPokemonNotFound()
        {
            var notPokemonName = "notPokemonName";
            var getPokemonUrl = GetPokemonEndpoint(notPokemonName);
            var response = await _httpClient.GetAsync(getPokemonUrl);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
