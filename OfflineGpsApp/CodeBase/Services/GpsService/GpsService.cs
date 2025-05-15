using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.GpsService
{
    public static class GpsService
    {
        private static double _lastLatitude = Double.MinValue;
        private static double _lastLongitude = Double.MinValue;
        private static DateTime _lastDateTime = DateTime.MinValue;

        /// <summary>
        /// Returns the last known coordinates from the GPS module with updating in specific timeout after last request
        /// </summary>
        /// <returns>Tuple<Longitude_double, Latitude_double></Longitude_double></returns>
        public static async Task<Tuple<double, double>> GetLastKnownCoordinates3857(int secondsInterval = 30)
        {
            // Checking if the last known coordinates are still valid to secondsInterval
            DateTime nowDateTime = DateTime.Now;
            if (_lastDateTime != DateTime.MinValue && (nowDateTime - _lastDateTime).TotalSeconds < secondsInterval)
            {
                System.Diagnostics.Debug.WriteLine($"OK: GpsService: GetLastKnownCoordinates3857: using cached coordinates {_lastLatitude}, {_lastLongitude} - Time: {_lastDateTime} - Passed: {(nowDateTime - _lastDateTime).TotalSeconds} seconds at limit of 60");
                return new Tuple<double, double>(_lastLatitude, _lastLongitude);
            }

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var oLocation = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLocationAsync(request);

            if (oLocation == null)
            {
                oLocation = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLastKnownLocationAsync();
            }

            if (oLocation != null)
            {
                if ((DateTimeOffset.UtcNow - oLocation.Timestamp).TotalMinutes < 5)
                {
                    System.Diagnostics.Debug.WriteLine($"OK: GpsService: GetLastKnownCoordinates3857: fresh coordinates got {oLocation.Latitude}, {oLocation.Longitude} - Time: {oLocation.Timestamp}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"OK: GpsService: GetLastKnownCoordinates3857: old coordinates got {oLocation.Latitude}, {oLocation.Longitude} - Time: {oLocation.Timestamp}");
                }

                _lastLatitude = oLocation.Latitude;
                _lastLongitude = oLocation.Longitude;
                _lastDateTime = nowDateTime;

                return new Tuple<double, double>(oLocation.Latitude, oLocation.Longitude);


            }

            System.Diagnostics.Debug.WriteLine($"ER: GpsService: GetLastKnownCoordinates3857: didn't got coordinates from GPS module");
            return new Tuple<double, double>(0, 0);
        }
    }
}
