using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders.Helpers
{
    /// <summary>
    /// Class that helps to create GPX tracks
    /// </summary>
    //public class GpxTrackBuilderHelper
    //{
    //    /// <summary>
    //    /// Читает GPX-файл и возвращает границы (мин/макс широта/долгота)
    //    /// </summary>
    //    /// <param name="gpxPath">Путь к GPX-файлу</param>
    //    /// <returns>Кортеж (minLat, maxLat, minLon, maxLon)</returns>
    //    private async System.Threading.Tasks.Task<(double minLat, double maxLat, double minLon, double maxLon)> GetGpxBoundsAsync(string? gpxFileContent)
    //    {
    //        double minLat = double.MaxValue;
    //        double maxLat = double.MinValue;
    //        double minLon = double.MaxValue;
    //        double maxLon = double.MinValue;

    //        try
    //        {
    //            // Читаем файл как строку
    //            //string gpxFileContent = await System.IO.File.ReadAllTextAsync(gpxPath);

    //            GpxParserService oGpxParserService = new GpxParserService();
    //            System.Collections.Generic.List<List<string>> AllLatLonListList = oGpxParserService.process_fc(gpxFileContent);

    //            foreach (List<string> latLonList in AllLatLonListList)
    //            {
    //                if (latLonList.Count == 2)
    //                {
    //                    if (double.TryParse(latLonList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
    //                        double.TryParse(latLonList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon))
    //                    {
    //                        minLat = System.Math.Min(minLat, lat);
    //                        maxLat = System.Math.Max(maxLat, lat);
    //                        minLon = System.Math.Min(minLon, lon);
    //                        maxLon = System.Math.Max(maxLon, lon);
    //                    }
    //                }
    //            }

    //            // Если границы не найдены, возвращаем (0, 0, 0, 0)
    //            if (minLat == double.MaxValue || maxLat == double.MinValue || minLon == double.MaxValue || maxLon == double.MinValue)
    //            {
    //                return (0, 0, 0, 0);
    //            }

    //            return (minLat, maxLat, minLon, maxLon);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            await this.DisplayAlert("Ошибка", $"Не удалось прочитать GPX: {ex.Message}", "OK");
    //            return (0, 0, 0, 0);
    //        }
    //    }
    //}
}
