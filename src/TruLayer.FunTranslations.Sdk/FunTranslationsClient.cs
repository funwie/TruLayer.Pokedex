using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TruLayer.FunTranslations.Sdk.Models;

namespace TruLayer.FunTranslations.Sdk
{
    public class FunTranslationsClient : IFunTranslationsClient
    {
        private readonly HttpClient _httpClient;
        private readonly FunTranslationsSettings _funTranslationsSettings;

        public FunTranslationsClient(HttpClient httpClient, IOptions<FunTranslationsSettings> options)
        {
            _httpClient = httpClient;
            _funTranslationsSettings = options.Value;
        }

        public async Task<TranslationResponse> Translate(string text, string translation, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrWhiteSpace(translation)) throw new ArgumentNullException(nameof(translation));

            var textTranslationUri = $"{_funTranslationsSettings.BaseUrl}/translate/{translation}";
            var translationRequest = new { Text = text };

            var response = await _httpClient.PostAsJsonAsync(textTranslationUri, translationRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TranslationResponse>(cancellationToken: cancellationToken);
            }

            return null;
        }
    }
}