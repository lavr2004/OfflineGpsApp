namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models
{
    /// <summary>
    /// Class that represents a point on the map
    /// </summary>
    public class MapsuiPointClass
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? IconPath { get; set; }
        public string? IconName { get; set; }
        public string? IconDescription { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.MinValue;

        public bool IsOk { get; set; } = true;

        public MapsuiPointClass(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

    }
}
