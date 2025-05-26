using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Microsoft.Maui.Controls;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models
{
    /// <summary>
    /// Class that represents a point on the map
    /// </summary>
    public class MapsuiServicePointModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? IconPath { get; set; }
        public string? IconName { get; set; }
        public string? IconDescription { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.MinValue;

        public bool IsOk { get; set; } = true;

        public MapsuiServicePointModel(double latitude, double longitude, double elevation = Double.MinValue)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public MapsuiServicePointModel(string latitude, string longitude, string elevation = "")
        {
            if (double.TryParse(latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
             double.TryParse(longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
            {
                Latitude = lat;
                Longitude = lon;
                if (!String.IsNullOrEmpty(elevation))
                {
                    double.TryParse(elevation, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double Elevation);
                }
            }
            else
            {
                throw new ArgumentException("Invalid latitude or longitude format");
            }
        }

        public Mapsui.IFeature ToMapsuiFeature()
        {
            //.ToMPoint() - is Mapsui.Extensions method
            //Mapsui.IFeature feature = new PointFeature(SphericalMercator.FromLonLat(lon, lat).ToMPoint());
            //feature["name"] = c.Name;
            return new PointFeature(SphericalMercator.FromLonLat(this.Longitude, this.Latitude).ToMPoint());
        }
    }
}