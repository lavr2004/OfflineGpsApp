using OfflineGpsApp.CodeBase.Service.LayersService;
using OfflineGpsApp.CodeBase.Service.GpxParserService;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI.Maui;
using Mapsui.Layers;
using System.Reflection.Metadata;
using Mapsui.Extensions;
using OfflineGpsApp.CodeBase.Services.MapsuiService.Builders;
using Mapsui.Nts;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Mapsui.Nts.Extensions;
//using SharpGPX;

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
        Mapsui.Tiling.Layers.TileLayer oTileLayer = TileLayerFactory.CreateTileLayer(isUseOnlineTiles: false);

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


        //Microsoft.Maui.Storage.FileSystem.AppDataDirectory - /data/data/com.yourapp/files/
        string gpxPath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "route.gpx");
        if (!System.IO.File.Exists(gpxPath))
        {
            // Создаём тестовый GPX-файл
            string gpxContent = @"<?xml version='1.0' encoding='utf-8'?>
<ns0:gpx xmlns:ns0=""http://www.topografix.com/GPX/1/1"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" creator=""Garmin Connect"" version=""1.1"" xsi:schemaLocation=""http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/11.xsd"">
  <ns0:metadata>
    <ns0:name>Hiking_Warszawa-Kampinos_40km_FreeMindsWay</ns0:name>
    <ns0:link href=""connect.garmin.com"">
      <ns0:text>Garmin Connect</ns0:text>
    </ns0:link>
    <ns0:time>2025-04-21T09:55:29.000Z</ns0:time>
  </ns0:metadata>
  <ns0:trk>
    <ns0:name>Hiking_Warszawa-Kampinos_40km_FreeMindsWay</ns0:name>
    <ns0:trkseg>
      <ns0:trkpt lat=""52.31022119522095"" lon=""20.76190710067749"">
        <ns0:ele>86.86</ns0:ele>
        <ns0:time>2025-04-21T09:55:29.000Z</ns0:time>
      </ns0:trkpt>
      <ns0:trkpt lat=""52.34967875294387"" lon=""20.303464019671082"">
        <ns0:ele>79.66</ns0:ele>
        <ns0:time>2025-04-21T20:33:21.080Z</ns0:time>
      </ns0:trkpt>
    </ns0:trkseg>
  </ns0:trk></ns0:gpx>";
            try
            {
                //remove old file
                if (System.IO.File.Exists(gpxPath))
                {
                    System.IO.File.Delete(gpxPath);
                }
                await System.IO.File.WriteAllTextAsync(gpxPath, gpxContent);
            }
            catch (System.Exception ex)
            {
                await this.DisplayAlert("Ошибка", $"Не удалось создать GPX: {ex.Message}", "OK");
                return;
            }

        }

        //step 2: Add GPX layer
        //parsing GPX file data +
        //create GPX points bounds to center screen that part of map + 
        //make track from trackpoints
        //create GPX layer
        //set GPX layer to map
        //show GPX layer on map

        (double minLat, double maxLat, double minLon, double maxLon) = await GetGpxBoundsAsync(gpxPath);
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

        string fileContent = await System.IO.File.ReadAllTextAsync(gpxPath);

        GpxParserService oGpxParserService = new GpxParserService();
        System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_fc(fileContent);

        IEnumerable<Mapsui.IFeature?> features = AllLatLonListList.Select(latLonList =>
        {
            if (latLonList.Count == 2)
            {
                if (double.TryParse(latLonList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(latLonList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                {
                    //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
                    //feature["name"] = c.Name;
                    return new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
                }
            }
            return null;
        }).Where(feature => feature != null);

        LayersBuilder oLayersBuilder = new LayersBuilder();
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

        var lineStringLayer = oLayersBuilder.CreateLineStringLayerFromLatLonList(AllLatLonListList);
        oMapsuiMap.Layers.Add(lineStringLayer);
        oMapsuiMap.Home = n => n.CenterOnAndZoomTo(lineStringLayer.Extent!.Centroid, 200);
        //oMapsuiMap.Refresh();
    }

    /// <summary>
    /// Читает GPX-файл и возвращает границы (мин/макс широта/долгота)
    /// </summary>
    /// <param name="gpxPath">Путь к GPX-файлу</param>
    /// <returns>Кортеж (minLat, maxLat, minLon, maxLon)</returns>
    private async System.Threading.Tasks.Task<(double minLat, double maxLat, double minLon, double maxLon)> GetGpxBoundsAsync(string gpxPath)
    {
        double minLat = double.MaxValue;
        double maxLat = double.MinValue;
        double minLon = double.MaxValue;
        double maxLon = double.MinValue;

        try
        {
            // Читаем файл как строку
            string fileContent = await System.IO.File.ReadAllTextAsync(gpxPath);

            GpxParserService oGpxParserService = new GpxParserService();
            System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_fc(fileContent);

            foreach (List<string> latLonList in AllLatLonListList)
            {
                if (latLonList.Count == 2)
                {
                    if (double.TryParse(latLonList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(latLonList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    {
                        minLat = System.Math.Min(minLat, lat);
                        maxLat = System.Math.Max(maxLat, lat);
                        minLon = System.Math.Min(minLon, lon);
                        maxLon = System.Math.Max(maxLon, lon);
                    }
                }
            }

            // Если границы не найдены, возвращаем (0, 0, 0, 0)
            if (minLat == double.MaxValue || maxLat == double.MinValue || minLon == double.MaxValue || maxLon == double.MinValue)
            {
                return (0, 0, 0, 0);
            }

            return (minLat, maxLat, minLon, maxLon);
        }
        catch (System.Exception ex)
        {
            await this.DisplayAlert("Ошибка", $"Не удалось прочитать GPX: {ex.Message}", "OK");
            return (0, 0, 0, 0);
        }
    }

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
