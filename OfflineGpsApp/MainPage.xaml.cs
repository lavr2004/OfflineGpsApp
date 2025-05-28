using Mapsui;
using Mapsui.Projections;
using OfflineGpsApp.CodeBase.Services.GpxParserService;
using OfflineGpsApp.CodeBase.Services.GpsService;
using OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter;

namespace OfflineGpsApp;

public partial class MainPage : ContentPage
{
    Mapsui.Map oMapsuiMap;
    OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServiceMapModel oMapsuiServiceMapModel;

    private readonly IGpsServiceAdapter _gpsService;//added for GPS service

    public MainPage(IGpsServiceAdapter gpsServiceFromSpecificPlatform)
    {
        _gpsService = gpsServiceFromSpecificPlatform;//added for GPS service
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
        System.Tuple<double, double> LatLonTuple = Task.Run(async () => await GpsService.GetLastKnownCoordinates3857()).Result;
        CenterMapOnPoint(oMapsuiMap, latitude: LatLonTuple.Item1, longitude: LatLonTuple.Item2);

        MapViewXaml.Map = oMapsuiMap;

        MapViewXaml.MyLocationLayer.UpdateMyLocation(new Mapsui.UI.Maui.Position(LatLonTuple.Item1, LatLonTuple.Item2), animated: true);
        MapViewXaml.RefreshData();
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

        //requesting permissions to read from local filesystem
        bool hasPermission = await OfflineGpsApp.CodeBase.App.Settings.RequestPermissionsSettings.RequestStoragePermission();
        if (!hasPermission)
        {
            await DisplayAlert("ERROR", "No have granted access to storage", "ОК");
            return;
        }

        string? gpxContent;

        // Call the dialog to open GPX file from local storage
        gpxContent = await OfflineGpsApp.CodeBase.App.Settings.FileSystemSettings.ReadGpxFileContentFromFileSystemAndroid();
        if (gpxContent != null)
        {
            await DisplayAlert("OK", $"File choosen successfully", "ОК");
        }
        else
        {
            return;
        }

        // parsing GPX STRING file content into SERIALIZED MapsuiServiceTrackModel object
        GpxParserService oGpxParserService = new GpxParserService();
        OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServiceTrackModel track;
        track = oGpxParserService.process_parse_trackpoints_from_gpx_into_trackmodel(gpxContent);


        // Adding layer with TRACK ROUTE LINE string from GPX track to the map
        Mapsui.Layers.ILayer lineStringLayer = track.ToLineStringLayer("LineStringLayer");
        oMapsuiMap.Layers.Add(lineStringLayer);
        oMapsuiMap.Home = n => n.CenterOnAndZoomTo(lineStringLayer.Extent!.Centroid, 200);


        // Adding layer with WAYPOINTS from GPX track to the map as a separate layer
        Mapsui.Layers.MemoryLayer gpxTrackPointsMemoryLayer = track.ToLayerWayPoints("GPXTrackPointsLayer");
        oMapsuiMap.Layers.Add(gpxTrackPointsMemoryLayer);
        

        // Centering map on bounds of the GPX track
        (double minLat, double maxLat, double minLon, double maxLon) = await track.GetGpxTrackBoundsAsync();
        await this.DisplayAlert("GPX Bounds", $"MinLat: {minLat}, MaxLat: {maxLat}, MinLon: {minLon}, MaxLon: {maxLon}", "OK");
        if (minLat == 0 && maxLat == 0 && minLon == 0 && maxLon == 0)
        {
            await this.DisplayAlert("ER", "Impossible to exctract coordinates bounds from the track", "OK");
            return;
        }
        else
        {
            CenterMapOnBounds(minLat, maxLat, minLon, maxLon);
        }

        // refresh content on the map
        oMapsuiMap.RefreshData();
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

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        //var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        var status = await OfflineGpsApp.CodeBase.App.Settings.RequestPermissionsSettings.RequestGpsLocationPermission();

        if (status != true)
        {
            await DisplayAlert("ERROR", "Required permissions to access GPS module", "OK");
            return;
        }

        Tuple<double, double> LatLonTuple = await GpsService.GetLastKnownCoordinates3857();
        System.Diagnostics.Debug.WriteLine($"OK: MainPage: OnAppearing: Last known gps location: {LatLonTuple.Item1}, {LatLonTuple.Item2}");
        MapViewXaml.RefreshData();


        // Updating user Gps Marker Location on the map
        MapViewXaml.MyLocationLayer.UpdateMyLocation(new Mapsui.UI.Maui.Position(LatLonTuple.Item1, LatLonTuple.Item2), true);
        MapViewXaml.RefreshData();

        // subscribe to changed location updates for adding to GPS service
        _gpsService.LocationChanged += (sender, e) =>
        {
            var newLatitude = e.Latitude;
            var newLongitude = e.Longitude;
            MapViewXaml.MyLocationLayer.UpdateMyLocation(new Mapsui.UI.Maui.Position(newLatitude, newLongitude), true);
            MapViewXaml.RefreshData();
            CenterMapOnPoint(oMapsuiMap, newLatitude, newLongitude, zoomlevel: 14);
        };

        _gpsService.StartListening();
    }

    /// <summary>
    /// Added for GPS service unsubscribing
    /// </summary>
    protected override void OnDisappearing()
    {
        _gpsService.StopListening();
        _gpsService.LocationChanged -= (sender, e) => { };
    }
}
