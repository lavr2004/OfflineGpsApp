using BruTile;
using Mapsui.Tiling.Layers;

namespace OfflineGpsApp.CodeBase.Service.LayersService
{
    public static class TileLayerFactory
    {
        public static TileLayer CreateTileLayer(bool isUseOnlineTiles)
        {
            TileLayer? oTileLayer;

            if (isUseOnlineTiles)
            {
                oTileLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
                oTileLayer.Name = "Online OSM Tiles";
            }
            else
            {
                ITileSource oITileSource = new OpenStreetMapLocalTileSource();
                oTileLayer = new TileLayer(oITileSource);
                oTileLayer.Name = "Local OSM Tiles";
            }
            return oTileLayer;
        }
    }
}
