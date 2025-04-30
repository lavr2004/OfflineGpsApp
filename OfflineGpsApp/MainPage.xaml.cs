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
//using SharpGPX;


namespace OfflineGpsApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        SetupMap();
    }

    private void SetupMap()
    {
        //Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
        //Mapsui.Tiling.Layers.TileLayer oTileLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();

        //using OfflineGpsApp.CodeBase.Service.LayersService;
        //Mapsui.Tiling.Layers.TileLayer oTileLayer = new Mapsui.Tiling.Layers.TileLayer(new OpenStreetMapLocalTileSource());

        Mapsui.Tiling.Layers.TileLayer oTileLayer = TileLayerFactory.CreateTileLayer(isUseOnlineTiles: true);

        Mapsui.Map oMapsuiMap = new Mapsui.Map()
        {
            CRS = "EPSG:3857", // Spherical Mercator projection
        };
        oMapsuiMap.Layers.Add(oTileLayer);

        CenterMapOnPoint(oMapsuiMap, latitude: 51.5, longitude: 0);
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

    //private async void OnLoadGpxClicked(object sender, System.EventArgs e)
    //{
    //    string gpxPath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "route.gpx");
    //    if (!System.IO.File.Exists(gpxPath))
    //    {
    //        await DisplayAlert("Ошибка", "GPX-файл не найден", "OK");
    //        return;
    //    }

    //    (double minLat, double maxLat, double minLon, double maxLon) = await GetGpxBoundsAsync(gpxPath);
    //    await DisplayAlert("GPX Bounds", $"MinLat: {minLat}, MaxLat: {maxLat}, MinLon: {minLon}, MaxLon: {maxLon}", "OK");
    //}
    private async void OnLoadGpxClicked(object sender, System.EventArgs e)
    {
        //Microsoft.Maui.Storage.FileSystem.AppDataDirectory - /data/data/com.yourapp/files/
        string gpxPath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "route.gpx");
        if (!System.IO.File.Exists(gpxPath))
        {
            // Создаём тестовый GPX-файл
            string gpxContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <gpx version=""1.1"" creator=""Test"">
                  <wpt lat=""51.5"" lon=""0.0"">
                    <name>Point 1</name>
                  </wpt>
                  <trk>
                    <name>Test Track</name>
                    <trkseg>
                      <trkpt lat=""51.5"" lon=""0.0""></trkpt>
                      <trkpt lat=""51.6"" lon=""0.1""></trkpt>
                    </trkseg>
                  </trk>
                </gpx>";
            try
            {
                await System.IO.File.WriteAllTextAsync(gpxPath, gpxContent);
            }
            catch (System.Exception ex)
            {
                await this.DisplayAlert("Ошибка", $"Не удалось создать GPX: {ex.Message}", "OK");
                return;
            }
        }

        (double minLat, double maxLat, double minLon, double maxLon) = await GetGpxBoundsAsync(gpxPath);
        await this.DisplayAlert("GPX Bounds", $"MinLat: {minLat}, MaxLat: {maxLat}, MinLon: {minLon}, MaxLon: {maxLon}", "OK");
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

            GpxParserService oGpxParserService = new GpxParserService(fileContent);
            System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.AllLatLonListList;

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
}
