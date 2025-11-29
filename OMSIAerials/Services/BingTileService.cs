using Newtonsoft.Json;
using OMSIAerials.Controllers;
using OMSIAerials.Interfaces;

namespace OMSIAerials.Services;

public class BingTileService : IMapTileService
{
    public string ProviderName { get; } = "Bing";

    private readonly HttpClient _http = new();

    public async Task<string> GetTileUriAsync(TileRequest request, CancellationToken ct = default)
    {
        var requestUrl = $"http://dev.virtualearth.net/REST/V1/Imagery/Metadata/{request.Tileset}?mapVersion=v1&output=json&key={request.ApiCode}";

        var response = await _http.GetAsync(requestUrl, ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Initial fetch failed: {(int)response.StatusCode} {response.ReasonPhrase}");

        var jsonResponse = await response.Content.ReadAsStringAsync(ct);
        dynamic? data = JsonConvert.DeserializeObject(jsonResponse);

        if (null == data)
            throw new InvalidOperationException("Initial response returned invalid content.");

        // extract subdomains from response
        var bingUrl = (string)data.resourceSets[0].resources[0].imageUrl;
        var subdomains = data.resourceSets[0].resources[0].imageUrlSubdomains;

        // pick a random subdomain
        var index = Random.Shared.Next(0, subdomains.Count);
        var subdomain = subdomains[index].ToString();

        return bingUrl.Replace("{subdomain}", subdomain).Replace("{quadkey}", ToQuadKey(request.X, request.Y, request.Z));
    }

    /// <summary>
    /// Converts tile XY coordinates and a level of detail into a quadkey string that uniquely identifies a tile in a
    /// quadtree structure.
    /// </summary>
    /// <param name="tileX">The X coordinate of the tile. Must be in the range [0, 2^levelOfDetail - 1].</param>
    /// <param name="tileY">The Y coordinate of the tile. Must be in the range [0, 2^levelOfDetail - 1].</param>
    /// <param name="levelOfDetail">The level of detail, which defines the depth of the quadtree. Must be a positive integer.</param>
    /// <returns>A string representing the quadkey for the specified tile coordinates and level of detail.</returns>
    private static string ToQuadKey(int tileX, int tileY, int levelOfDetail)
    {
        var quadKey = string.Empty;
        for (int i = levelOfDetail; i > 0; i--)
        {
            int digit = 0;
            int mask = 1 << (i - 1);
            if ((tileX & mask) != 0)
                digit++;

            if ((tileY & mask) != 0)
                digit += 2;

            quadKey += digit.ToString();
        }

        return quadKey;
    }
}