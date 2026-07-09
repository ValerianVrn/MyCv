using MyCv.Tailor.Api.Models;

namespace MyCv.Tailor.Api.Services
{
    internal class TailorService(IGeminiClient geminiClient) : ITailorService
    {
        private readonly IGeminiClient _geminiClient = geminiClient;

        public Task<TailorResult> TailorAsync(string input)
        {
            return _geminiClient.GenerateAsync(input);
        }
    }
}
