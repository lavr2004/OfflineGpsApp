using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Helpers;

public static class GeoCalculationsHelper
{
    const double R = 6371000; // Earth radius in meters

    /// <summary>
    /// Haversine formula for calculation distance between points on Sphere in meters: haversine(θ) = sin²(θ/2)
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public static double HaversineDistanceMeters(MapsuiServiceTrackPointModel point1, MapsuiServiceTrackPointModel point2)
    {
        double lat1 = point1.Latitude * System.Math.PI / 180;
        double lat2 = point2.Latitude * System.Math.PI / 180;
        double deltaLat = (point2.Latitude - point1.Latitude) * System.Math.PI / 180;
        double deltaLon = (point2.Longitude - point1.Longitude) * System.Math.PI / 180;

        double a = System.Math.Sin(deltaLat / 2) * System.Math.Sin(deltaLat / 2) +
                   System.Math.Cos(lat1) * System.Math.Cos(lat2) *
                   System.Math.Sin(deltaLon / 2) * System.Math.Sin(deltaLon / 2);
        double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
        return R * c; // Distance in meters
    }
}
