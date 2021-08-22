using System.Threading;
using System.Threading.Tasks;
using TruLayer.FunTranslations.Sdk.Models;

namespace TruLayer.FunTranslations.Sdk
{
    public interface IFunTranslationsClient
    {
        Task<TranslationResponse> Translate(string text, string translation, CancellationToken cancellationToken);
    }
}
