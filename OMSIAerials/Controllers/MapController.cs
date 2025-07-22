using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OMSIAerials.Model;

namespace OMSIAerials.Controllers;

[Route("aerial")]
[ApiController]
public class MapController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Provider provider, [FromQuery] string apiCode, [FromQuery] string type, [FromQuery] int x, [FromQuery] int y, [FromQuery] int z)
    {
        return provider switch
        {
            Provider.Bing => await GetBingAerial(apiCode, type, x, y, z),
            Provider.Google => GetGoogleAerial(apiCode, type, x, y, z),
            Provider.Mapbox => GetMapboxAerial(apiCode, type, x, y, z),
            _ => Content("Invalid provider.")
        };
    }

    private async Task<IActionResult> GetBingAerial(string apiCode, string type, int x, int y, int z)
    {
        // Bing REST URL
        var requestUrl = $"http://dev.virtualearth.net/REST/V1/Imagery/Metadata/{type}?mapVersion=v1&output=json&key={apiCode}";

        using var client = new HttpClient();
        var response = await client.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
            return Content("Error: " + (int)response.StatusCode + " " + response.ReasonPhrase);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(jsonResponse);

        // Extract image URL and subdomains from the response
        var bingUrl = (string)data.resourceSets[0].resources[0].imageUrl;
        var subdomains = data.resourceSets[0].resources[0].imageUrlSubdomains;

        // Pick a random subdomain
        var r = new Random();
        var index = r.Next(0, subdomains.Count);
        var subdomain = subdomains[index].ToString();

        return Redirect(
                bingUrl.Replace("{subdomain}", subdomain).Replace("{quadkey}", ToQuadKey(x, y, z))
            );
    }

    // type is satellite or roadmap
    private RedirectResult GetGoogleAerial(string apiCode, string type, int x, int y, int z)
    {

        string res, scale;
        //if (!string.IsNullOrEmpty(Request.Query["hres"]))
        //{
        //    string hres = Request.Query["hres"];
        //    if (hres == "1")
        //    {
        //        scale = "2";
        //        res = "256x256";
        //    }
        //    else if (hres == "2")
        //    {
        //        res = "512x512";
        //        scale = "1";
        //    }
        //    else
        //    {
        //        res = "256x256";
        //        scale = "1";
        //    }
        //}
        //else
        {
            res = "256x256";
            scale = "1";
        }

        string format;// = Request.Query["format"];
                      //if (string.IsNullOrEmpty(format))
                      //{
        format = "png";
        //}

        // Generate the center latitude and longitude using the helper function
        string center = ToCoordinates(x, y, z);
        string baseUrl = $"http://maps.googleapis.com/maps/api/staticmap?center={center}&maptype={type}&zoom={z}&size={res}&scale={scale}&sensor=false&format={format}&key={apiCode}";

        return Redirect(baseUrl);
    }



    private RedirectResult GetMapboxAerial(string apiCode, string type, int x, int y, int z)
    {
        Console.WriteLine($"x = {x}; y = {y}; zoom = {z}");

        var url = $"https://api.mapbox.com/styles/v1/mapbox/satellite-v9/static/{ToCoordinatesInv(x, y, z)},17,0/512x512?access_token={apiCode}";
        return Redirect(url);
    }

    //        else if (provider.Equals("yandex", StringComparison.OrdinalIgnoreCase))
    //{
    //    string type = Request.Query["type"];
    //    if (string.IsNullOrEmpty(type))
    //    {
    //        type = "sat";
    //    }

    //    int x = int.Parse(Request.Query["x"]);
    //    int y = int.Parse(Request.Query["y"]);
    //    int z = int.Parse(Request.Query["z"]);

    //    string center = ToLatLong(x, y, z);
    //    string baseUrl = $"http://static-maps.yandex.ru/1.x/?lang=en-US&ll={center}&z={z}&l={type}&size=256,256";

    //    return Redirect(baseUrl);
    //}

    //return Content("Invalid service provider.");

    // Helper function to compute the quad key based on tile coordinates and level of detail

    private static string ToQuadKey(int tileX, int tileY, int levelOfDetail)
    {
        string quadKey = "";
        for (int i = levelOfDetail; i > 0; i--)
        {
            int digit = 0;
            int mask = 1 << (i - 1);
            if ((tileX & mask) != 0)
            {
                digit++;
            }
            if ((tileY & mask) != 0)
            {
                digit += 2;
            }
            quadKey += digit.ToString();
        }

        return quadKey;
    }

    private static string ToCoordinates(int x, int y, int z)
    {
        var n = Math.Pow(2, z);
        var lon_deg = (x + 0.5) / n * 360.0 - 180.0;
        var lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (y + 0.5) / n)));
        var lat_deg = lat_rad * (180.0 / Math.PI);

        return $"{lat_deg},{lon_deg}";
    }

    private static string ToCoordinatesInv(int x, int y, int z)
    {
        var n = Math.Pow(2, z);
        var lon_deg = (x + 0.5) / n * 360.0 - 180.0;
        var lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (y + 0.5) / n)));
        var lat_deg = lat_rad * (180.0 / Math.PI);

        return $"{lon_deg},{lat_deg}";
    }
}