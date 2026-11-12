using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MyCv.UI.Wasm;
using MyCv.UI.Wasm.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Custom services
builder.Services
    .AddScoped<IThemeService, ThemeService>()
    .AddScoped<ILocalizationService, LocalizationService>();

// Configuration.
_ = builder.Configuration.GetValue<bool>("TailorApi:UseFake")
    ? builder.Services.AddScoped<ITailorService, FakeTailorService>()
    : builder.Services.AddScoped<ITailorService, TailorService>();

// MudBlazor services
builder.Services.AddMudServices();

// Localization (https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?#dynamically-set-the-client-side-culture-by-user-preference)
builder.Services.AddLocalization();

var host = builder.Build();

var js = host.Services.GetRequiredService<IJSRuntime>();
// Stored choice, else browser language (see wwwroot/scripts/culture.js).
var cultureName = await js.InvokeAsync<string>("blazorCulture.current");
CultureInfo culture;
try
{
    culture = CultureInfo.GetCultureInfo(cultureName);
}
catch (CultureNotFoundException)
{
    culture = LocalizationService.English;
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
