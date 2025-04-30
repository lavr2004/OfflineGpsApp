using BruTile.Predefined;
using BruTile;

namespace OfflineGpsApp.CodeBase.Service.LayersService
{
    public class OpenStreetMapLocalTileSource : ITileSource//BruTile
    {
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
            return null;
        }
    }
}
