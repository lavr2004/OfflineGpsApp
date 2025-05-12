using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.GpxTrackBuilder
{
    /// <summary>
    /// Class that helps to create GPX tracks
    /// </summary>
    public class GpxTrackBuilderHelper
    {
        /// <summary>
        /// Provides calculations of the bounds for the GPX track
        /// </summary>
        /// <param name="MapsuiServicePointModelList">List of point models from our custom MapsuiService</param>
        /// <returns>Tuple of (minLat, maxLat, minLon, maxLon)</returns>
        public static async System.Threading.Tasks.Task<(double minLat, double maxLat, double minLon, double maxLon)> GetGpxBoundsAsync(System.Collections.Generic.List<MapsuiServicePointModel> MapsuiServicePointModelList)
        {
            double minLat = double.MaxValue;
            double maxLat = double.MinValue;
            double minLon = double.MaxValue;
            double maxLon = double.MinValue;

            foreach (MapsuiServicePointModel oMapsuiServicePointModel in MapsuiServicePointModelList)
            {
                minLat = System.Math.Min(minLat, oMapsuiServicePointModel.Latitude);
                maxLat = System.Math.Max(maxLat, oMapsuiServicePointModel.Latitude);
                minLon = System.Math.Min(minLon, oMapsuiServicePointModel.Longitude);
                maxLon = System.Math.Max(maxLon, oMapsuiServicePointModel.Longitude);
            }

            // If bounds didnt found (0, 0, 0, 0)
            if (minLat == double.MaxValue || maxLat == double.MinValue || minLon == double.MaxValue || maxLon == double.MinValue)
            {
                return (0, 0, 0, 0);
            }

            return (minLat, maxLat, minLon, maxLon);
        }
    }
}
