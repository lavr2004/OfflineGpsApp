using Android.Content;
using Android.Locations;
using Android.OS;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

using nsAndroid = Android;
using OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter;

namespace OfflineGpsApp.Platforms.Android;

public class GpsServiceAdapterAndroid : Java.Lang.Object, IGpsServiceAdapter, nsAndroid.Locations.ILocationListener
{
    private nsAndroid.Locations.LocationManager _locationManager;
    public event System.EventHandler<GpsServiceAdapterEventArgs> LocationChanged;

    public GpsServiceAdapterAndroid()
    {
        _locationManager = (nsAndroid.Locations.LocationManager)Microsoft.Maui.ApplicationModel.Platform.AppContext.GetSystemService(nsAndroid.Content.Context.LocationService);
    }

    public void StartListening()
    {
        if (CheckSelfPermission(nsAndroid.Manifest.Permission.AccessFineLocation) == (int)nsAndroid.Content.PM.Permission.Granted)
        {
            _locationManager.RequestLocationUpdates(nsAndroid.Locations.LocationManager.GpsProvider, minTimeMs: 5000, minDistanceM: 0, listener: this);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("ER - GpsServiceAdapterAndroid - Location permission not granted");
        }
    }

    public void StopListening()
    {
        _locationManager.RemoveUpdates(listener:this);
    }

    public void OnLocationChanged(nsAndroid.Locations.Location location)
    {
        System.Diagnostics.Debug.WriteLine($"OK - GpsServiceAdapterAndroid - Location updated: Latitude={location.Latitude}, Longitude={location.Longitude}");
        LocationChanged?.Invoke(this, new GpsServiceAdapterEventArgs
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude
        });
        
    }

    public void OnProviderDisabled(string provider) { }
    public void OnProviderEnabled(string provider) { }
    public void OnStatusChanged(string provider, nsAndroid.Locations.Availability status, nsAndroid.OS.Bundle extras) { }

    // Проверка разрешения (добавлено для безопасности)
    private int CheckSelfPermission(string permission)
    {
        return (int)(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.CheckSelfPermission(permission) ?? nsAndroid.Content.PM.Permission.Denied);
    }
}