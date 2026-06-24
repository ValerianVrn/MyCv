using MudBlazor.Services;
using MyCv.UI.Components;
using MyCv.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddLocalization()
    .AddControllers();
builder.Services.AddMudServices();
builder.Services
    .AddScoped<IRatingService, FakeRatingService>()
    .AddScoped<IInsightService, FakeInsightService>()
    .AddScoped<IVisitorService, VisitorService>()
    .AddScoped<IThemeService, ThemeService>()
    .AddScoped<ILocalizationService, LocalizationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


// Localization
var supportedCultures = new[] { LocalizationService.English.Name, LocalizationService.French.Name };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[1])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

//localizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
