using Microsoft.Extensions.DependencyInjection;

namespace TruLayer.PokeApi.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPokeApiClient(this IServiceCollection services)
        {
            services.AddHttpClient<IPokeApiClient, PokeApiClient>();
        }
    }
}
