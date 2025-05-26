using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

/// <summary>
/// Class that represents a track on the map
/// </summary>
public class MapsuiServiceTrackModel
{
    List<MapsuiServicePointModel> oMapsuiPointModelList;

    public List<MapsuiServicePointModel> OMapsuiPointModelList
    {
        get { return oMapsuiPointModelList; }
    }

    public MapsuiServiceTrackModel(List<MapsuiServicePointModel> oMapsuiPointModelList = null)
    {
        if (oMapsuiPointModelList == null)
        {
            oMapsuiPointModelList = new List<MapsuiServicePointModel>();
        }
        else
        {
            this.oMapsuiPointModelList = oMapsuiPointModelList;
        }
    }

    public void AddPointToTrack(MapsuiServicePointModel oMapsuiPointClass)
    {
        oMapsuiPointModelList.Add(oMapsuiPointClass);
    }

    /// <summary>
    /// Provides calculations of the bounds for the GPX track
    /// </summary>
    /// <returns>Tuple of (minLat, maxLat, minLon, maxLon)</returns>
    public async System.Threading.Tasks.Task<(double minLat, double maxLat, double minLon, double maxLon)> GetGpxTrackBoundsAsync()
    {
        double minLat = double.MaxValue;
        double maxLat = double.MinValue;
        double minLon = double.MaxValue;
        double maxLon = double.MinValue;

        foreach (MapsuiServicePointModel oMapsuiServicePointModel in oMapsuiPointModelList)
        {
            minLat = System.Math.Min(minLat, oMapsuiServicePointModel.Latitude);
            maxLat = System.Math.Max(maxLat, oMapsuiServicePointModel.Latitude);
            minLon = System.Math.Min(minLon, oMapsuiServicePointModel.Longitude);
            maxLon = System.Math.Max(maxLon, oMapsuiServicePointModel.Longitude);
        }

        // If bounds didnt found (0, 0, 0, 0)
        if (minLat == double.MaxValue || maxLat == double.MinValue || minLon == double.MaxValue || maxLon == double.MinValue)
        {
            return (0, 0, 0, 0);
        }

        return (minLat, maxLat, minLon, maxLon);
    }

    /// <summary>
    /// Creates a list of features for the track points
    /// start - finish - middle waypoints on the track
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Mapsui.IFeature> ToWaypointFeaturesOnTheTrack()
    {
        System.Collections.Generic.List<MapsuiServicePointModel> waypointsMapsuiServicePointModelList = new();
        waypointsMapsuiServicePointModelList.Add(oMapsuiPointModelList[0]);//start point
        waypointsMapsuiServicePointModelList.Add(oMapsuiPointModelList[oMapsuiPointModelList.Count - 1]);//finish point

        IEnumerable<Mapsui.IFeature> features = waypointsMapsuiServicePointModelList.Select(oMapsuiServicePointModel =>
        {
            //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
            //feature["name"] = c.Name;
            return oMapsuiServicePointModel.ToMapsuiFeature();
        }).Where(feature => feature != null);

        return features;
    }

    public Mapsui.Layers.MemoryLayer ToLayerWayPoints(string layerTitle = "TrackWaypointsLayer")
    {
        IEnumerable<Mapsui.IFeature> oPointFeatures = this.ToWaypointFeaturesOnTheTrack();

        if (oPointFeatures == null || oPointFeatures.Count() == 0) throw new ArgumentException("ER - oPointFeatures is null or empty");
        return new MemoryLayer()
        {
            Name = layerTitle,//"MarkersLayer",
            IsMapInfoLayer = true,
            Features = new MemoryProvider(oPointFeatures).Features,
            Style = Mapsui.Styles.SymbolStyles.CreatePinStyle(symbolScale: 0.7),//create style for every pin on layer
        };
    }

    public Mapsui.Layers.ILayer ToLineStringLayer(string layerTitle = "TrackLineStringLayer")
    {
        //var lineString = (LineString)new WKTReader().Read(WKTGr5);
        //lineString = new LineString(lineString.Coordinates.Select(v => SphericalMercator.FromLonLat(v.Y, v.X).ToCoordinate()).ToArray());

        NetTopologySuite.Geometries.LineString oLineString = new LineString(oMapsuiPointModelList.Select(v => SphericalMercator.FromLonLat(v.Longitude, v.Latitude).ToCoordinate()).ToArray());//.ToCoordinate() - is extension method from Mapsui.Nts.Extensions

        return new MemoryLayer
        {
            Features = new[] { new GeometryFeature { Geometry = oLineString } },
            Name = layerTitle,//"LineStringLayer"
            Style = OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.StylesBuilder.StylesBuilder.CreateLineStringStyle()//create style for every line on layer

        };
    }
}
