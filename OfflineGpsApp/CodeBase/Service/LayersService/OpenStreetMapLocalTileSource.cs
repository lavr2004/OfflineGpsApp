//using nsBruTilePredefined = BruTile.Predefined;
//using nsBrutile = BruTile;

//using nsMapsuiStyles = Mapsui.Styles; //ADDED: for Mapsui Attribution
//using nsSystemNetHttp = System.Net.Http; //ADDED: for downloading tiles
//using nsSystemDiagnostics = System.Diagnostics; //ADDED: for logging

using BruTile;
using System.Diagnostics;

namespace OfflineGpsApp.CodeBase.Service.LayersService
{
    public class OpenStreetMapLocalTileSource : BruTile.ITileSource//BruTile
    {
        //BruTile.ITileSource interface requirements that TileLayer needs to implement
        public BruTile.ITileSchema Schema { get; } = new BruTile.Predefined.GlobalSphericalMercator();
        public BruTile.Attribution Attribution { get; } = new BruTile.Attribution("© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright"); // Исправлено: правильный конструктор BruTile.Attribution
        public System.String Name { get; } = "Local OSM Tiles";

        //ADDED: for downloading tiles
        private readonly HttpClient _httpClient;

        //ADDED: xUnit: for xUnit testing on different platforms
        private readonly string _tileCacheDirectory; //ADDED: store tile cache directory

        public OpenStreetMapLocalTileSource(string tileCacheDirectory = null)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "OfflineGpsApp/1.0 (contact: lavr2004@gmail.com)");
            _tileCacheDirectory = tileCacheDirectory ?? Path.Combine(FileSystem.AppDataDirectory, "tiles"); //CHANGED: xUnit: use parameter or default to AppDataDirectory
        }

        public async System.Threading.Tasks.Task<byte[]?> GetTileAsync(BruTile.TileInfo oTileInfo)
        {
            //path: tiles/0/0/0.png
            System.String tilePath = Path.Combine(_tileCacheDirectory, "tiles", oTileInfo.Index.Level.ToString(), oTileInfo.Index.Col.ToString(), $"{oTileInfo.Index.Row}.png");//CHANGED: xUnit
            //System.String tilePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "tiles", oTileInfo.Index.Level.ToString(), oTileInfo.Index.Col.ToString(), $"{oTileInfo.Index.Row}.png");
            System.Diagnostics.Debug.WriteLine($"OK - Checking tile at: {tilePath}"); //ADDED: for debugging tile path

            if (System.IO.File.Exists(tilePath))
            {
                System.Diagnostics.Debug.WriteLine($"OK - Reading local tile: {tilePath}"); //ADDED: for debugging
                return await System.IO.File.ReadAllBytesAsync(tilePath);
            }

            //ADDED: for downloading tiles from OSM
            System.Diagnostics.Debug.WriteLine($"OK - Local tile not found, downloading: {tilePath}"); //ADDED: for downloading tiles
            System.String url = $"https://tile.openstreetmap.org/{oTileInfo.Index.Level}/{oTileInfo.Index.Col}/{oTileInfo.Index.Row}.png"; //ADDED
            try //ADDED
            {
                System.Byte[] tileData = await _httpClient.GetByteArrayAsync(url); //ADDED
                System.Diagnostics.Debug.WriteLine($"OK - Downloaded tile: {url}"); //ADDED: for debugging
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(tilePath)); //ADDED
                await System.IO.File.WriteAllBytesAsync(tilePath, tileData); //ADDED
                System.Diagnostics.Debug.WriteLine($"OK - Saved tile to: {tilePath}"); //ADDED: for debugging
                return tileData; //ADDED
            } //ADDED
            catch (System.Net.Http.HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                System.Diagnostics.Debug.WriteLine($"ER - Forbidden (403) for tile {url}: Check User-Agent or OSM Tile Usage Policy.");
                return null;
            }
            catch (System.Exception ex) //ADDED
            {
                System.Diagnostics.Debug.WriteLine($"ER - Failed to download tile {url}: {ex.Message}"); //ADDED: for error logging
                return null; //ADDED
            } //ADDED
        }
    }
}
