using Microsoft.AspNetCore.Mvc;
using OMSIAerials.Interfaces;

namespace OMSIAerials.Controllers;

public record TileRequest(string ApiCode, string Tileset, int X, int Y, int Z);

[Route("aerial")]
[ApiController]
public class MapController(IEnumerable<IMapTileService> tileServices) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string provider, [FromQuery] TileRequest request, CancellationToken ct = default)
    {
        var svc = tileServices.FirstOrDefault(s => string.Equals(s.ProviderName, provider, StringComparison.OrdinalIgnoreCase));
        if (svc == null)
            return BadRequest($"Unknown provider '{provider}'.");

        try
        {
            var uri = await svc.GetTileUriAsync(request, ct);
            return Redirect(uri);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Problem($"Provider '{provider}' failed to produce a tile URL: {ex.Message}");
        }
    }
}