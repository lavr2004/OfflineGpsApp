using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Reflection.Metadata;

using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI.Maui;
using Mapsui.Layers;
using Mapsui.Extensions;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Mapsui.Nts.Extensions;
using OfflineGpsApp.CodeBase.Services.GpxParserService;
using OfflineGpsApp.CodeBase.Services.MapsuiService;
using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;
using OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.GpxTrackBuilder;

namespace OfflineGpsApp;

public partial class MainPage : ContentPage
{
    Mapsui.Map oMapsuiMap;

    public MainPage()
    {
        InitializeComponent();
        SetupMap();
    }

    private void SetupMap()
    {
        Mapsui.Tiling.Layers.TileLayer oTileLayer = OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.TileSourceBuilder.TileSourceBuilder.CreateTileLayer(isUseOnlineTiles: false);

        oMapsuiMap = new Mapsui.Map()
        {
            CRS = "EPSG:3857", // Spherical Mercator projection
        };
        oMapsuiMap.Layers.Add(oTileLayer);

        CenterMapOnPoint(oMapsuiMap, latitude: 51.5, longitude: 0);

        //step 2: Add GPX layer
        //parsing GPX file data
        //make track from trackpoints
        //create GPX layer
        //set GPX layer to map
        //show GPX layer on map

        MapViewXaml.Map = oMapsuiMap;
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

    private async void OnLoadGpxClicked(object sender, System.EventArgs e)
    {
        //step 2: Add GPX layer
        //parsing GPX file data +
        //create GPX points bounds to center screen that part of map
        //make track from trackpoints
        //create GPX layer
        //set GPX layer to map
        //show GPX layer on map

        string? gpxContent;

        
        try
        {
            gpxContent = await OfflineGpsApp.CodeBase.App.Settings.FileSystemSettings.GetReadStringFromMauiAsset("20250421_HIKING_WARSZAWA-KAMPINOS.gpx");
        }
        catch (System.Exception ex)
        {
            await this.DisplayAlert("Ошибка", $"Не удалось прочитать GPX: {ex.Message}", "OK");
            return;
        }

        //step 2: Add GPX layer
        //parsing GPX file data +
        //create GPX points bounds to center screen that part of map + 
        //make track from trackpoints
        //create GPX layer
        //set GPX layer to map
        //show GPX layer on map

        //(double minLat, double maxLat, double minLon, double maxLon) = await GetGpxBoundsAsync(gpxPath);
        GpxParserService oGpxParserService = new GpxParserService();
        //System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_parse_trackpoints_from_gpx(gpxContent);
        System.Collections.Generic.List<MapsuiServicePointModel> trackpointsMapsuiServicePointModelList = oGpxParserService.process_parse_trackpoints_from_gpx(gpxContent);
        //System.Collections.Generic.List<MapsuiServicePointModel> waypointsMapsuiServicePointModelList = oGpxParserService.process_parse_waypoints_from_gpx(gpxContent);
        System.Collections.Generic.List<MapsuiServicePointModel> waypointsMapsuiServicePointModelList = new();
        waypointsMapsuiServicePointModelList.Add(trackpointsMapsuiServicePointModelList[50]);
        waypointsMapsuiServicePointModelList.Add(trackpointsMapsuiServicePointModelList[100]);


        //(double minLat, double maxLat, double minLon, double maxLon) = await GetGpxBoundsAsync(gpxContent);
        (double minLat, double maxLat, double minLon, double maxLon) = await GpxTrackBuilderHelper.GetGpxBoundsAsync(trackpointsMapsuiServicePointModelList);
        await this.DisplayAlert("GPX Bounds", $"MinLat: {minLat}, MaxLat: {maxLat}, MinLon: {minLon}, MaxLon: {maxLon}", "OK");
        if (minLat == 0 && maxLat == 0 && minLon == 0 && maxLon == 0)
        {
            await this.DisplayAlert("Ошибка", "Не удалось извлечь координаты из GPX", "OK");
            return;
        }

        // Центрируем карту на регион GPX
        CenterMapOnBounds(minLat, maxLat, minLon, maxLon);

        //step 2: Add GPX layer
        //parsing GPX file data +
        //create GPX points bounds to center screen that part of map + 
        //create layer with points on map +++
        //show up GPX layer with points +++
        //make GPX track from trackpoints on map
        //show up GPX track from trackpoints on map
        //set GPX layer to map
        //show GPX layer on map

        //string fileContent = await System.IO.File.ReadAllTextAsync(gpxPath);


        //conversion into features on the map
        //IEnumerable<Mapsui.IFeature?> features = AllLatLonListList.Select(latLonList =>
        //{
        //    if (latLonList.Count == 2)
        //    {
        //        if (double.TryParse(latLonList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
        //            double.TryParse(latLonList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
        //        {
        //            //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
        //            //feature["name"] = c.Name;
        //            return new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
        //        }
        //    }
        //    return null;
        //}).Where(feature => feature != null);

        IEnumerable<Mapsui.IFeature?> features = waypointsMapsuiServicePointModelList.Select(oMapsuiServicePointModel =>
        {
            //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
            //feature["name"] = c.Name;
            return oMapsuiServicePointModel.ToMapsuiFeature();
        }).Where(feature => feature != null);


        OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.LayersBuilder.LayersBuilder oLayersBuilder = new OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.LayersBuilder.LayersBuilder();
        MemoryLayer gpxTrackPointsMemoryLayer = oLayersBuilder.CreateMemoryLayer(features, "GPXTrackPointsLayer");
        oMapsuiMap.Layers.Add(gpxTrackPointsMemoryLayer);
        oMapsuiMap.RefreshData();

        //step 2: Add GPX layer
        //parsing GPX file data +
        //create GPX points bounds to center screen that part of map + 
        //create layer with points on map +
        //show up GPX layer with points +
        //make GPX track from trackpoints on map +++
        //show up GPX track from trackpoints on map +++
        //set GPX layer to map +++
        //show GPX layer on map +++

        var lineStringLayer = oLayersBuilder.CreateLineStringLayerFromMapsuiServicePointModelList(trackpointsMapsuiServicePointModelList, LayerTitle: "LineStringLayer");
        oMapsuiMap.Layers.Add(lineStringLayer);
        oMapsuiMap.Home = n => n.CenterOnAndZoomTo(lineStringLayer.Extent!.Centroid, 200);
        //oMapsuiMap.Refresh();
    }

    ///// <summary>
    ///// Читает GPX-файл и возвращает границы (мин/макс широта/долгота)
    ///// </summary>
    ///// <param name="gpxPath">Путь к GPX-файлу</param>
    ///// <returns>Кортеж (minLat, maxLat, minLon, maxLon)</returns>
    //private async System.Threading.Tasks.Task<(double minLat, double maxLat, double minLon, double maxLon)> GetGpxBoundsAsync(string? gpxFileContent)
    //{
    //    double minLat = double.MaxValue;
    //    double maxLat = double.MinValue;
    //    double minLon = double.MaxValue;
    //    double maxLon = double.MinValue;

    //    try
    //    {
    //        // Читаем файл как строку
    //        //string gpxFileContent = await System.IO.File.ReadAllTextAsync(gpxPath);

    //        GpxParserService oGpxParserService = new GpxParserService();
    //        System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_parse_trackpoints_from_gpx(gpxFileContent);

    //        foreach (List<string> latLonList in AllLatLonListList)
    //        {
    //            if (latLonList.Count == 2)
    //            {
    //                if (double.TryParse(latLonList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
    //                    double.TryParse(latLonList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
    //                {
    //                    minLat = System.Math.Min(minLat, lat);
    //                    maxLat = System.Math.Max(maxLat, lat);
    //                    minLon = System.Math.Min(minLon, lon);
    //                    maxLon = System.Math.Max(maxLon, lon);
    //                }
    //            }
    //        }

    //        // Если границы не найдены, возвращаем (0, 0, 0, 0)
    //        if (minLat == double.MaxValue || maxLat == double.MinValue || minLon == double.MaxValue || maxLon == double.MinValue)
    //        {
    //            return (0, 0, 0, 0);
    //        }

    //        return (minLat, maxLat, minLon, maxLon);
    //    }
    //    catch (System.Exception ex)
    //    {
    //        await this.DisplayAlert("Ошибка", $"Не удалось прочитать GPX: {ex.Message}", "OK");
    //        return (0, 0, 0, 0);
    //    }
    //}

    /// <summary>
    /// Method to center the map on the given bounds calculated area of the GPX file
    /// </summary>
    /// <param name="oMapsuiMap"></param>
    /// <param name="minLat"></param>
    /// <param name="maxLat"></param>
    /// <param name="minLon"></param>
    /// <param name="maxLon"></param>
    private void CenterMapOnBounds(double minLat, double maxLat, double minLon, double maxLon)
    {
        var (minX, minY) = SphericalMercator.FromLonLat(minLon, maxLat);
        var (maxX, maxY) = SphericalMercator.FromLonLat(maxLon, minLat);

        oMapsuiMap.Navigator.ZoomToBox(new MRect(minX, minY, maxX, maxY)); //ADDED: to update map view immediately
        MapViewXaml.Map.RefreshData();
    }
}
