using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MyCv.UI.Wasm;
using MyCv.UI.Wasm.Services;
using System.Globalization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Custom services
builder.Services
    .AddScoped<IVisitorService, VisitorService>()
    .AddScoped<IThemeService, ThemeService>()
    .AddScoped<ILocalizationService, LocalizationService>();

// MudBlazor services
builder.Services.AddMudServices();

// Localization (https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?#dynamically-set-the-client-side-culture-by-user-preference)
builder.Services.AddLocalization();

var host = builder.Build();

var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(result ?? LocalizationService.French.Name);

if (result == null)
{
    await js.InvokeVoidAsync("blazorCulture.set", LocalizationService.French.Name);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
