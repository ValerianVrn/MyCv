using MyCv.Tailor.Api.Models;

namespace MyCv.Tailor.Api.Services;

internal interface ITailorService
{
    public Task<TailorResult> TailorAsync(string input);
}
