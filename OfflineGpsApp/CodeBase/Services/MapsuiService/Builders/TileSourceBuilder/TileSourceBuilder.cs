using BruTile;
using Mapsui.Tiling.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.TileSourceBuilder
{
    public class TileSourceBuilder
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
                ITileSource oITileSource = new MapsuiServiceLocalTileSourceModel();
                oTileLayer = new TileLayer(oITileSource);
                oTileLayer.Name = "Local OSM Tiles";
            }
            return oTileLayer;
        }
    }
}
