using OMSIAerials.Controllers;

namespace OMSIAerials.Interfaces;

public interface IMapTileService
{
    /// <summary>Logical provider identifier.</summary>
    string ProviderName { get; }

    /// <summary>Returns a final absolute URI to redirect the client to, or throws if unavailable.</summary>
    Task<string> GetTileUriAsync(TileRequest request, CancellationToken ct = default);
}
