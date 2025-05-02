using BruTile.Predefined;
using BruTile;

using Mapsui.Styles; //ADDED: for Mapsui Attribution
using System.Net.Http; //ADDED: for downloading tiles

namespace OfflineGpsApp.CodeBase.Service.LayersService
{
    public class OpenStreetMapLocalTileSource : ITileSource//BruTile
    {
        //ADDED: for downloading tiles
        private readonly HttpClient _httpClient = new HttpClient(); //ADDED

        //BruTile.ITileSource interface requirements that TileLayer needs to implement
        public ITileSchema Schema { get; } = new GlobalSphericalMercator();
        public string Name { get; } = "Local OSM Tiles";
        public Attribution Attribution { get; } = new Attribution("© OpenStreetMap");

        public async Task<byte[]?> GetTileAsync(TileInfo oTileInfo)
        {
            //path: tiles/0/0/0.png
            var tilePath = Path.Combine(FileSystem.AppDataDirectory, "tiles", oTileInfo.Index.Level.ToString(), oTileInfo.Index.Col.ToString(), $"{oTileInfo.Index.Row}.png");

            if (File.Exists(tilePath))
            {
                return await File.ReadAllBytesAsync(tilePath);
            }

            //ADDED: for downloading tiles from OSM
            var url = $"https://tile.openstreetmap.org/{oTileInfo.Index.Level}/{oTileInfo.Index.Col}/{oTileInfo.Index.Row}.png"; //ADDED
            try //ADDED
            {
                var tileData = await _httpClient.GetByteArrayAsync(url); //ADDED
                Directory.CreateDirectory(Path.GetDirectoryName(tilePath)); //ADDED
                await File.WriteAllBytesAsync(tilePath, tileData); //ADDED
                return tileData; //ADDED
            } //ADDED
            catch (Exception) //ADDED
            {
                return null; //ADDED
            } //ADDED
        }
    }
}
