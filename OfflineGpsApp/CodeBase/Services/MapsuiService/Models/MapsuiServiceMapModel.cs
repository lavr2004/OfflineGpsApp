using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nsMapsuiService = OfflineGpsApp.CodeBase.Services.MapsuiService;
using nsGpsService = OfflineGpsApp.CodeBase.Services.GpsService;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

/// <summary>
/// Model of Map
/// </summary>
public class MapsuiServiceMapModel
{
    Mapsui.Map oMapsuiMap;
    public Mapsui.Map OMapsuiMap
    {
        get { return oMapsuiMap; }
        set { oMapsuiMap = value; }
    }

    public MapsuiServiceMapModel()
    {
        oMapsuiMap = new Mapsui.Map(){ CRS = "EPSG:3857", };// Spherical Mercator projection
        SetupMap();
    }

    private void SetupMap()
    {
        Mapsui.Tiling.Layers.TileLayer oTileLayer = OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.TileSourceBuilder.TileSourceBuilder.CreateTileLayer(isUseOnlineTiles: false);
        oMapsuiMap.Layers.Add(oTileLayer);
        System.Tuple<double, double> LatLonTuple = Task.Run(async () => await nsGpsService.GpsService.GetLastKnownCoordinates3857()).Result;
        CenterMapOnPoint(oMapsuiMap, latitude: LatLonTuple.Item1, longitude: LatLonTuple.Item2);
    }

    /// <summary>
    /// Centering Map on point in the layer
    /// (London coordinates (51.5, 0) - (latitude, longitude) in 4326 (GPS) system of coordinates)
    /// </summary>
    /// <param name="oMapsuiMap"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <param name="zoomlevel"> min -> max == 0 -> 20 </param>
    public void CenterMapOnPoint(Mapsui.Map oMapsuiMap, double latitude = 51.5, double longitude = 0, int zoomlevel = 14)
    {
        var (x, y) = Mapsui.Projections.SphericalMercator.FromLonLat(longitude, latitude);//reverse order for Mercator Point (CRS)
        oMapsuiMap.Home = navigator => navigator.CenterOnAndZoomTo(new Mapsui.MPoint(x, y), navigator.Resolutions[zoomlevel]);
    }
}
