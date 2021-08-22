using Microsoft.Extensions.DependencyInjection;

namespace TruLayer.FunTranslations.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFunTranslationsClient(this IServiceCollection services)
        {
            services.AddHttpClient<IFunTranslationsClient, FunTranslationsClient>();
            return services;
        }
    }
}
