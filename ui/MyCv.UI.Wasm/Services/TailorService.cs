using MyCv.UI.Wasm.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyCv.UI.Wasm.Services
{
    /// <inheritdoc/>
    internal sealed class TailorService(HttpClient httpclient, IConfiguration configuration) : ITailorService
    {
        /// <summary>
        /// Must match <c>TailorFunction.MaxInputLength</c> in the Function App.
        /// </summary>
        public const int MaxInputLength = 4000;

        /// <inheritdoc/>
        public async Task<TailorResult?> TailorAsync(string input, CancellationToken ct = default)
        {
            try
            {
                var apiUrl = configuration["TailorApi:Url"] ?? "http://localhost:7071/api/tailor";
                using var response = await httpclient.PostAsJsonAsync(apiUrl, new { input }, ct);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new TailorRateLimitedException();
                }

                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TailorResult>(cancellationToken: ct);
            }
            catch (HttpRequestException ex)
            {
                throw new TailorUnavailableException("Tailor API call failed", ex);
            }
            catch (JsonException ex)
            {
                throw new TailorUnavailableException("Tailor API returned invalid JSON", ex);
            }
        }
    }
}
