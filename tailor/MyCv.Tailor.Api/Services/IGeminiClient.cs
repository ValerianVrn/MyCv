using MyCv.Tailor.Api.Models;

namespace MyCv.Tailor.Api.Services;

internal interface IGeminiClient
{
    public Task<TailorResult> GenerateAsync(string input);
}
