using System;
using System.Collections.Generic;
using Mapsui.Projections;

using nsDatabaseService = OfflineGpsApp.CodeBase.Services.DatabaseService;
using nsMapsuiService = OfflineGpsApp.CodeBase.Services.MapsuiService;

//ADDED: for calculating distance from point to track and filtering monuments by distance, adapted for MapsuiServiceTrackPointModel
namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Helpers
{
    public class TrackProximityFilter
    {
        public System.Collections.Generic.List<nsDatabaseService.Models.DatabaseServiceMonumentModel> FilterMonumentsByDistance(
            System.Collections.Generic.List<nsDatabaseService.Models.DatabaseServiceMonumentModel> monuments,
            System.Collections.Generic.List<nsMapsuiService.Models.MapsuiServiceTrackPointModel> trackPoints,
            System.Double maxDistanceMeters,
            System.IProgress<System.Double> progress = null) //ADDED: progress reporting
        {
            System.Diagnostics.Debug.WriteLine($"Filtering {monuments.Count} monuments with {trackPoints.Count} track points"); //ADDED: log input sizes
            var filteredMonuments = new System.Collections.Generic.List<nsDatabaseService.Models.DatabaseServiceMonumentModel>(); //ADDED: to store filtered monuments
            for (int i = 0; i < monuments.Count; i++)
            {
                var monument = monuments[i];
                var monumentPoint = new nsMapsuiService.Models.MapsuiServiceTrackPointModel(monument.Latitude, monument.Longitude); //ADDED: convert monument to track point model
                if (IsPointNearTrack(monumentPoint, trackPoints, maxDistanceMeters))
                {
                    filteredMonuments.Add(monument); //ADDED: include monument if within distance
                    System.Diagnostics.Debug.WriteLine($"Monument {monument.Name} is within {maxDistanceMeters}m"); //ADDED: log included monument
                }

                ////ADDED: report progress bar
                //if (progress != null)
                //{
                //    double progressPercentage = (i + 1) / (double)monuments.Count;
                //    progress.Report(progressPercentage); //ADDED: report progress as fraction
                //}
            }
            return filteredMonuments;
        }

        private System.Boolean IsPointNearTrack(
            nsMapsuiService.Models.MapsuiServiceTrackPointModel point,
            System.Collections.Generic.List<nsMapsuiService.Models.MapsuiServiceTrackPointModel> trackPoints,
            System.Double maxDistanceMeters)
        {
            for (int i = 0; i < trackPoints.Count - 1; i++)
            {
                var p1 = trackPoints[i];
                var p2 = trackPoints[i + 1];
                var distance = DistanceToSegment(point, p1, p2);
                if (distance <= maxDistanceMeters)
                {
                    return true; //ADDED: point is within max distance
                }
            }
            return false;
        }

        private System.Double DistanceToSegment(
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p,
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p1,
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p2)
        {
            //ADDED: calculate distance to closest point on segment using Haversine formula
            var closest = ClosestPointOnSegment(p, p1, p2);
            return HaversineDistance(p, closest);
        }

        private nsMapsuiService.Models.MapsuiServiceTrackPointModel ClosestPointOnSegment(
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p,
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p1,
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p2)
        {
            //ADDED: find closest point on segment using Mercator projection coordinates (X, Y)
            var dx = p2.X - p1.X; //CHANGED: use Mercator X for linear calculations
            var dy = p2.Y - p1.Y; //CHANGED: use Mercator Y for linear calculations
            if (dx == 0 && dy == 0)
            {
                return p1; //ADDED: return p1 if segment is a point
            }

            var t = ((p.X - p1.X) * dx + (p.Y - p1.Y) * dy) / (dx * dx + dy * dy); //CHANGED: use Mercator coordinates for projection
            t = System.Math.Max(0, System.Math.Min(1, t));

            var closestX = p1.X + t * dx; //CHANGED: calculate closest point in Mercator X
            var closestY = p1.Y + t * dy; //CHANGED: calculate closest point in Mercator Y

            //ADDED: convert Mercator coordinates back to Latitude/Longitude
            (double lon, double lat) = Mapsui.Projections.SphericalMercator.ToLonLat(closestX, closestY);

            return new nsMapsuiService.Models.MapsuiServiceTrackPointModel(lat, lon); //CHANGED: create new track point model with converted coordinates
        }

        private System.Double HaversineDistance(
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p1,
            nsMapsuiService.Models.MapsuiServiceTrackPointModel p2)
        {
            //ADDED: calculate distance between two points using Haversine formula
            const System.Double R = 6371000; // Earth's radius in meters
            var lat1 = p1.Latitude * System.Math.PI / 180; //CHANGED: use Latitude from track point model
            var lat2 = p2.Latitude * System.Math.PI / 180; //CHANGED: use Latitude from track point model
            var deltaLat = (p2.Latitude - p1.Latitude) * System.Math.PI / 180; //CHANGED: use Latitude for delta
            var deltaLon = (p2.Longitude - p1.Longitude) * System.Math.PI / 180; //CHANGED: use Longitude for delta

            var a = System.Math.Sin(deltaLat / 2) * System.Math.Sin(deltaLat / 2) +
                    System.Math.Cos(lat1) * System.Math.Cos(lat2) *
                    System.Math.Sin(deltaLon / 2) * System.Math.Sin(deltaLon / 2);
            var c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
            return R * c;
        }
    }
}