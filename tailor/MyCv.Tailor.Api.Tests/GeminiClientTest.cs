using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MyCv.Tailor.Api.Services;
using System.Net;

namespace MyCv.Tailor.Api.Tests
{
    [TestClass]
    public sealed class GeminiClientTest
    {
        private const string InvalidGeminiResponse = /*lang=json,strict*/ """
            {
              "candidates": [
                {   
                  "content": {
                    "parts": [
                      {
                        "text": "{\n  \"match_case\": 3,\n  \"match_stars\": 5,\n  \"match_details\": {\n    \"role\": \"Valérian a une solide expérience en tant que lead developer, ayant managé des équipes et piloté des projets techniques de bout en bout.\",\n    \"sector\": \"Son parcours inclut des expériences significatives dans le secteur financier (Société Générale CIB) et au sein de startups dynamiques (Kili Technology), ce qui correspond parfaitement au contexte d'une startup fintech.\",\n    \"tech_stack\": \"Valérian maîtrise Azure, ainsi que d'autres plateformes cloud comme AWS et GCP, garantissant une expertise pertinente pour un rôle d'Azure tech lead.\"\n  }\n}"
                      }
                    ],
                    "role": "model"
                  },
                  "finishReason": "STOP",
                  "index": 0
                }
              ],
              "usageMetadata": {
                "promptTokenCount": 102,
                "candidatesTokenCount": 160,
                "totalTokenCount": 540,
                "promptTokensDetails": [
                  {
                    "modality": "TEXT",
                    "tokenCount": 102
                  }
                ],
                "thoughtsTokenCount": 278,
                "serviceTier": "standard"
              },
              "modelVersion": "gemini-2.5-flash",
              "responseId": "L7JLavbaFMP5xN8P2-fwwAE"
            }
            """;
        private const string ValidGeminiResponse = /*lang=json,strict*/ """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  {
                    "text": "{\"case\":3,\"stars\":5,\"matchLabel\":\"Strong match\",\"humor\":\"Congratulations — Valérian is pretty much your guy. (I don't say that every time, I promise.)\",\"whyMatch\":[\"Azure\",\"C#/.NET\",\"Microservices\",\"Tech Lead\"],\"skillBridges\":[{\"asked\":\"AWS\",\"have\":\"Azure\"}],\"bonusSkills\":[\"Event Sourcing\",\"AI integration\"],\"pitch\":\"10+ years building distributed systems on Azure, leading teams and shipping microservices at scale.\",\"contactCopy\":\"Convinced? Let's talk.\"}"
                  }
                ],
                "role": "model"
              },
              "finishReason": "STOP",
              "index": 0
            }
          ],
          "usageMetadata": {
            "promptTokenCount": 102,
            "candidatesTokenCount": 160,
            "totalTokenCount": 540,
            "promptTokensDetails": [
              {
                "modality": "TEXT",
                "tokenCount": 102
              }
            ],
            "thoughtsTokenCount": 278,
            "serviceTier": "standard"
          },
          "modelVersion": "gemini-2.5-flash",
          "responseId": "L7JLavbaFRP5xN8P2-fwwAE"
        }
        """;

        [TestMethod]
        public async Task InvalidClientResponse_GenerateAsync_ThrowsValidationException()
        {
            Environment.SetEnvironmentVariable(GeminiClient.GEMINIAPIKEY, "fake");
            var logger = new Mock<ILogger<GeminiClient>>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _ = mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(InvalidGeminiResponse) });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _ = httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var geminiClient = new GeminiClient(logger.Object, httpClientFactoryMock.Object);
            _ = await Assert.ThrowsExceptionAsync<ValidationException>(async () => await geminiClient.GenerateAsync(""));
        }

        [TestMethod]
        public async Task ValidClientResponse_GenerateAsync_ReturnsGeminiResponse()
        {
            Environment.SetEnvironmentVariable(GeminiClient.GEMINIAPIKEY, "fake");
            var logger = new Mock<ILogger<GeminiClient>>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _ = mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(ValidGeminiResponse) });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _ = httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var geminiClient = new GeminiClient(logger.Object, httpClientFactoryMock.Object);
            var geminiResponse = await geminiClient.GenerateAsync("");
            Assert.IsNotNull(geminiResponse);
            Assert.IsTrue(geminiResponse.Case is > 0 and <= 3);
            Assert.IsTrue(geminiResponse.Stars is >= 0 and <= 5);
            Assert.IsFalse(string.IsNullOrEmpty(geminiResponse.MatchLabel));
            Assert.IsFalse(string.IsNullOrEmpty(geminiResponse.Humor));
            Assert.IsFalse(string.IsNullOrEmpty(geminiResponse.Pitch));
            Assert.IsFalse(string.IsNullOrEmpty(geminiResponse.ContactCopy));
        }
    }
}
