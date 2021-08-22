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
using TruLayer.FunTranslations.Sdk.Models;

namespace TruLayer.FunTranslations.Sdk.Tests
{
    public class FunTranslationsClientTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Test]
        [Description(@"GIVEN a text and a translation
                       AND FunTranslations API will return success with content
                       WHEN Translate is called
                       THEN expected translation response is returned")]
        public async Task TranslationRequestIsSuccessful()
        {
            var responseFromApi = _fixture.Create<TranslationResponse>();
            var httpClientMock = HttpClientMock(HttpStatusCode.OK, responseFromApi);
            var funTranslationsSettings = new FunTranslationsSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(funTranslationsSettings);
            var sut = new FunTranslationsClient(httpClientMock, options);

            var expectedTranslationResponse = new TranslationResponse
            {
                Success = new Success
                {
                    Total = responseFromApi.Success.Total
                },
                Contents = new Contents
                {
                    Translated = responseFromApi.Contents.Translated,
                    Text = responseFromApi.Contents.Text,
                    Translation = responseFromApi.Contents.Translation
                }
            };

            var textToTranslate = _fixture.Create<string>();
            var translation = _fixture.Create<string>();
            var actualTranslationResponse = await sut.Translate(textToTranslate, translation, CancellationToken.None);

            actualTranslationResponse.Should().BeEquivalentTo(expectedTranslationResponse);
        }

        [Test]
        [Description(@"GIVEN a text and a translation
                       AND the request to Fun Translation API will fail
                       WHEN Translate is called
                       THEN null is returned")]
        public async Task TranslationRequestFails()
        {
            var httpClientMock = HttpClientMock(HttpStatusCode.BadRequest, null);
            var funTranslationsSettings = new FunTranslationsSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(funTranslationsSettings);

            var textToTranslate = _fixture.Create<string>();
            var translation = _fixture.Create<string>();
            var sut = new FunTranslationsClient(httpClientMock, options);

            var actualTranslationResponse = await sut.Translate(textToTranslate, translation, CancellationToken.None);

            actualTranslationResponse.Should().BeNull();
        }


        [TestCase(null, "yoda")]
        [TestCase("", "yoda")]
        [TestCase("hello", null)]
        [TestCase("hello", "")]
        [Description(@"GIVEN null or empty text/translation
                       WHEN Translate is called
                       THEN ArgumentNullException is thrown")]
        public async Task InvalidInput(string textToTranslate, string translation)
        {
            var responseFromApi = _fixture.Create<TranslationResponse>();
            var httpClientMock = HttpClientMock(HttpStatusCode.OK, responseFromApi);
            var funTranslationsSettings = new FunTranslationsSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(funTranslationsSettings);
            
            var sut = new FunTranslationsClient(httpClientMock, options);
            Func<Task> act = async () => await sut.Translate(textToTranslate, translation, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

       
        [Test]
        [Description(@"GIVEN a text and a translation
                       AND the request to Fun Translation API will throw an exception
                       WHEN Translate is called
                       THEN the exception is rethrown")]
        public async Task TranslationRequestThrowsException()
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            var httpClientMock = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };

            var funTranslationsSettings = new FunTranslationsSettings { BaseUrl = "http://localhost/" };
            var options = Options.Create(funTranslationsSettings);

            var textToTranslate = _fixture.Create<string>();
            var translation = _fixture.Create<string>();
            var sut = new FunTranslationsClient(httpClientMock, options);
            Func<Task> act = async () => await sut.Translate(textToTranslate, translation, CancellationToken.None);

            await act.Should().ThrowAsync<HttpRequestException>();
        }

        private static HttpClient HttpClientMock(HttpStatusCode statusCode, TranslationResponse responseContent = null)
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