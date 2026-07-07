using MyCv.UI.Wasm.Exceptions;
using System.Net.Http.Json;

namespace MyCv.UI.Wasm.Services
{
    /// <inheritdoc/>
    internal class TailorService(HttpClient httpclient, IConfiguration configuration) : ITailorService
    {
        /// <inheritdoc/>
        public async Task<TailorResult?> TailorAsync(string input, CancellationToken ct = default)
        {
            try
            {
                var apiUrl = configuration["TailorApi:Url"] ?? "http://localhost:7071/api/tailor";
                var response = await httpclient.PostAsJsonAsync(apiUrl, new { input }, ct);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TailorResult>(cancellationToken: ct);
            }
            catch (HttpRequestException)
            {
                throw new TailorUnavailableException();
            }
        }
    }
}
