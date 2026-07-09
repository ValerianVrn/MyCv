using MyCv.Tailor.Api.Models;

namespace MyCv.Tailor.Api.Services
{
    /// <summary>
    /// Calls the Gemini model to tailor the CV according to the user input.
    /// </summary>
    internal interface IGeminiClient
    {
        public Task<TailorResult> GenerateAsync(string input);
    }
}
