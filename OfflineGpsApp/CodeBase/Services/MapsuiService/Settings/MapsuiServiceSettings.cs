using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Settings
{
    public static class MapsuiServiceSettings
    {
        //callout settings on the map
        public const int CalloutTitleLengthMax = 10;

        //title of layer with pins on the map
        public const string TaskModelsLayerTitle = "MarkersLayer";

        //title of layer with tracks on the map
        public const string TracksLayerTitle = "TracksLayer";

        //public static bool isReturnFromMapPage = false;

        static double _lastLatitude = 0;
        static double _lastLongitude = 0;
        static DateTime _lastDateTime = DateTime.MinValue;

        public static double LastLatitude
        {
            get { return _lastLatitude; }
            set { _lastLatitude = value; }
        }

        public static double LastLongitude
        {
            get { return _lastLongitude; }
            set { _lastLongitude = value; }
        }

        public static DateTime LastDateTime
        {
            get { return _lastDateTime; }
            set { _lastDateTime = value; }
        }

        public static List<MapsuiServicePointModel> GetInitialMapsuiPointList()
        {
            return new List<MapsuiServicePointModel>()
            {
                new MapsuiServicePointModel(52.231, 21.001)
                {
                    Title = "Warsaw",
                    IconPath = "pin.png",
                    IconName = "pin.png",
                    IconDescription = "Warsaw pin",
                    Description = "Warsaw pin",
                    CreatedDateTime = DateTime.Now,
                    IsOk = true

                },
                new MapsuiServicePointModel(53.893009, 27.567444){
                    Title = "Minsk",
                    IconPath = "pin.png",
                    IconName = "pin.png",
                    IconDescription = "Minsk pin icon description",
                    Description = "Minsk pin content description",
                    CreatedDateTime = DateTime.Now.AddMinutes(1),
                    IsOk = false
                }
            };
        }

        /// <summary>
        /// Returns the last known coordinates from the GPS module with updating in specific timeout after last request
        /// </summary>
        /// <returns>Tuple<Longitude_double, Latitude_double></Longitude_double></returns>
        public static async Task<Tuple<double, double>> GetLastKnownCoordinates3857()
        {
            // Проверяем, есть ли свежие кэшированные координаты (менее 30 секунд)
            DateTime nowDateTime = DateTime.Now;
            if (_lastDateTime != DateTime.MinValue && (nowDateTime - _lastDateTime).TotalSeconds < 30)
            {
                Console.WriteLine($"OK: GlobalSettings: GetLastKnownCoordinates3857: using cached coordinates {_lastLatitude}, {_lastLongitude} - Time: {_lastDateTime} - Passed: {(nowDateTime - _lastDateTime).TotalSeconds} seconds at limit of 60");
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
                    Console.WriteLine($"OK: GlobalSettings: GetLastKnownCoordinates3857: fresh coordinates got {oLocation.Latitude}, {oLocation.Longitude} - Time: {oLocation.Timestamp}");
                }
                else
                {
                    Console.WriteLine($"OK: GlobalSettings: GetLastKnownCoordinates3857: old coordinates got {oLocation.Latitude}, {oLocation.Longitude} - Time: {oLocation.Timestamp}");
                }

                _lastLatitude = oLocation.Latitude;
                _lastLongitude = oLocation.Longitude;
                _lastDateTime = nowDateTime;

                return new Tuple<double, double>(oLocation.Latitude, oLocation.Longitude);


            }

            Console.WriteLine($"ER: GlobalSettings: GetLastKnownCoordinates3857: didn't got coordinates from GPS module");
            return new Tuple<double, double>(0, 0);
        }
    }
}
