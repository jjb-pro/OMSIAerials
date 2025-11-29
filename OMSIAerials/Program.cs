using Microsoft.FluentUI.AspNetCore.Components;
using OMSIAerials.Components;
using OMSIAerials.Interfaces;
using OMSIAerials.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();
builder.Services.AddControllers();
builder.Services
    .AddTransient<IMapTileService, BingTileService>()
    .AddTransient<IMapTileService, MapboxTileService>()
    .AddTransient<IMapTileService, GoogleTileService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();