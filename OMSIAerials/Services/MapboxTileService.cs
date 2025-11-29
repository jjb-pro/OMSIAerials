using OMSIAerials.Controllers;
using OMSIAerials.Interfaces;

namespace OMSIAerials.Services;

public class MapboxTileService : IMapTileService
{
    public string ProviderName { get; } = "Mapbox";

    public Task<string> GetTileUriAsync(TileRequest request, CancellationToken ct = default)
    {
        var tileset = request.Tileset switch
        {
            "Aerial" => "mapbox.satellite",
            "Road" => "mapbox.mapbox-streets-v8",
            "Terrain" => "mapbox.mapbox-terrain-v2",
            _ => throw new ArgumentOutOfRangeException(nameof(request), $"Tileset type '{request.Tileset}' is unknown.")
        };

        return Task.FromResult($"https://api.mapbox.com/v4/{tileset}/{request.Z}/{request.X}/{request.Y}.jpg90?access_token={request.ApiCode}");
    }
}