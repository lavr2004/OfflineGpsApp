using Mapsui;
using Mapsui.Projections;
using OfflineGpsApp.CodeBase.Services.GpxParserService;
using OfflineGpsApp.CodeBase.Services.GpsService;
using OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter;
using OsmSharp.API;
using System;

namespace OfflineGpsApp;

public partial class MainPage : ContentPage
{
    Mapsui.Map oMapsuiMap;
    OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServiceMapModel oMapsuiServiceMapModel;

    private readonly IGpsServiceAdapter _gpsService;//added for GPS service
    //private bool _isFiltering; //ADDED: track filtering state for PROGRESS BAR
    //public bool IsFiltering
    //{
    //    get => _isFiltering;
    //    set
    //    {
    //        _isFiltering = value;
    //        OnPropertyChanged(nameof(IsFiltering)); //ADDED: notify UI of filtering state change
    //    }
    //}

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
    /// Loads monuments and points of interest (POIs) that are close to the track points within a specified distance.
    /// </summary>
    /// <param name="trackPoints"></param>
    /// <param name="distanceMeters"></param>
    private async System.Threading.Tasks.Task<Mapsui.Layers.MemoryLayer> GetClosestMonumentsPois(
    System.Collections.Generic.List<OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServiceTrackPointModel> trackPoints,
    System.Double distanceMeters = 2000.0)
    {
        System.Diagnostics.Debug.WriteLine($"Starting GetClosestMonumentsPois with {trackPoints.Count} track points"); //ADDED: log track points count
        var dbService = new OfflineGpsApp.CodeBase.Services.DatabaseService.DatabaseService(); //CHANGED: no path required
        var monuments = dbService.GetMonuments(); //ADDED: load monuments from database
        if (monuments == null || monuments.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("ER: no monuments found in the database."); //ADDED: log empty result
            return new Mapsui.Layers.MemoryLayer()
            {
                Name = "MonumentsLayer",
            }; //ADDED: return empty layer if no monuments found
        }

        System.Diagnostics.Debug.WriteLine($"Found {monuments.Count} monuments in database"); //ADDED: log number of monuments
        foreach (var monument in monuments.Take(5)) //ADDED: log first 5 monuments for debugging
        {
            System.Diagnostics.Debug.WriteLine($"Monument: Id={monument.Id}, Name={monument.Name}, Latitude={monument.Latitude}, Longitude={monument.Longitude}");
        }

        var filter = new OfflineGpsApp.CodeBase.Services.MapsuiService.Helpers.TrackProximityFilter();
        //var filteredMonuments = filter.FilterMonumentsByDistance(monuments, trackPoints, distanceMeters); //ADDED: filter by distance
        //var progress = new System.Progress<System.Double>(progress =>
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        ProgressBar.Progress = progress; //ADDED: update progress bar
        //        ProgressLabel.Text = $"Filtering monuments: {(progress * 100):F1}%"; //ADDED: update label
        //    });
        //});

        //var filteredMonuments = await System.Threading.Tasks.Task.Run(() => filter.FilterMonumentsByDistance(monuments, trackPoints, distanceMeters, progress));//ADDED: optimization for HARD calculations
        var filteredMonuments = await System.Threading.Tasks.Task.Run(() => filter.FilterMonumentsByDistance(monuments, trackPoints, distanceMeters));//ADDED: optimization for HARD calculations


        System.Diagnostics.Debug.WriteLine($"Filtered {filteredMonuments.Count} monuments within {distanceMeters} meters"); //ADDED: log filtered count
        foreach (var monument in filteredMonuments)
        {
            System.Diagnostics.Debug.WriteLine($"OK - monument {monument.Name} - {monument.Latitude} - {monument.Longitude} - {monument.SourceIdentifier}");
        }

        var poiLayerModel = new OfflineGpsApp.CodeBase.Services.MapsuiService.Models.MapsuiServicePoiLayerModel(filteredMonuments, "MonumentsLayer");
        var poisLayer = poiLayerModel.GetPoisLayer(); //ADDED: get POI layer from model
        System.Diagnostics.Debug.WriteLine($"Created POI layer with {poisLayer.Features.Count()} features"); //ADDED: log feature count

        return poisLayer;
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
        //ADDED: show progress bar and disable UI interaction
        //IsFiltering = true;
        //ProgressBarStackLayout.IsVisible = true; // Show the progress bar
        //ProgressBar.Progress = 0;
        //ProgressLabel.Text = "Filtering POIS: 0%";


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

        //ADDED: show loading indicator
        IsBusy = true; //CHANGED: enable built-in loading indicator
        try
        {
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

            // Adding layer with POIs from GPX track to the map as a separate layer
            Mapsui.Layers.MemoryLayer closestPoisLayer = await GetClosestMonumentsPois(track.OMapsuiPointModelList, distanceMeters: 2000.0); //ADDED: load monuments and POIs that are close to the track points
            oMapsuiMap.Layers.Add(closestPoisLayer);


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
        finally
        {
            //IsFiltering = false; //ADDED: hide progress bar
            IsBusy = false;
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
