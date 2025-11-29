using Newtonsoft.Json;
using OMSIAerials.Controllers;
using OMSIAerials.Interfaces;
using System.Text;

namespace OMSIAerials.Services;

public class GoogleTileService : IMapTileService
{
    public string ProviderName { get; } = "Google";

    public async Task<string> GetTileUriAsync(TileRequest request, CancellationToken ct = default)
    {
        var mapType = request.Tileset switch
        {
            "Aerial" => "satellite",
            "Road" => "roadmap",
            _ => throw new ArgumentOutOfRangeException(nameof(request), $"Tileset type '{request.Tileset}' is unknown."),
        };

        // maybe add aerial with labels in future
        //case "AerialLabels":
        //mapType = "satellite";
        //layerTypes = ["layerRoadmap"];
        //break;

        // create session token
        var requestUrl = $"https://tile.googleapis.com/v1/createSession?key={request.ApiCode}";

        using var client = new HttpClient();
        var requestBody = new
        {
            mapType,
            language = "en-US",
            region = "US",
            //layerTypes
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUrl, content, ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Session token fetch failed: {(int)response.StatusCode} {response.ReasonPhrase}");

        var jsonResponse = await response.Content.ReadAsStringAsync(ct);
        if (null == jsonResponse)
            throw new InvalidOperationException("Session token fetch returned empty response.");

        dynamic data = JsonConvert.DeserializeObject(jsonResponse)!;
        var session = data.session.ToString();

        return $"https://tile.googleapis.com/v1/2dtiles/{request.Z}/{request.X}/{request.Y}?session={session}&key={request.ApiCode}";
    }
}