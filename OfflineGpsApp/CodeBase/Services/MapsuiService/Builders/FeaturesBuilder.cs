using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using OfflineGpsApp.CodeBase.Services.MapsuiService.Models;
using OfflineGpsApp.CodeBase.Services.MapsuiService.Settings;
//using OfflineGpsApp.CodeBase.MVVM.Model;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders;

/// <summary>
/// This class is used to create Mapsui.IFeature objects for the map based on the coordinates provided.
/// From GPS-coordinates (EPSG:4326) → to Mercator (EPSG:3857) coordinates.
/// EPSG:3857-coordinates.
/// Or based on MapsuiPointClass data object.
/// </summary>
public class FeaturesBuilder
{
    /// <summary>
    /// Getting up coordinates in Latitude and Longtitude GPS values (EPSG:4326), then transform that into EPSG:3857 point valuable for Geo Maps
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    public static IFeature CreateFeatureFromGpsCoordinates(double latitude, double longitude)
    {
        //Create EPSG:3857 point from GPS coordinates (EPSG:4326)
        var oMPoint = SphericalMercator.FromLonLat(longitude, latitude).ToMPoint();
        var feature = new PointFeature(oMPoint);
        return feature;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    public static IFeature CreateFeatureFromEPSG3857Coordinates(double MercatorLatitude, double MercatorLongitude)
    {
        //Create EPSG:4326 Map Point from GPS coordinates (EPSG:3857)
        Mapsui.MPoint oMPoint = new Mapsui.MPoint(x: MercatorLatitude, y: MercatorLongitude);
        var feature = new PointFeature(oMPoint);
        return feature;
    }

    /// <summary>
    /// Create feature for EPSG:3857 from MapsuiPointClass Latitude and Longitude posted in GPS coordinates (EPSG:4326)
    /// </summary>
    /// <param name="oMapsuiPointClass">universal data class used in MapsuiService</param>
    /// <returns></returns>
    public static IFeature CreateFeatureFromMapsuiPointClass(MapsuiPointClass oMapsuiPointClass)
    {
        //MapsuiPointClass keeps GPS coordinates in EPSG:4326 - that it is x, y
        Mapsui.IFeature feature = CreateFeatureFromGpsCoordinates(oMapsuiPointClass.Latitude, oMapsuiPointClass.Longitude);

        if (oMapsuiPointClass.Title.Length > MapsuiServiceSettings.CalloutTitleLengthMax)
        {
            feature[nameof(oMapsuiPointClass.Title)] = oMapsuiPointClass.Title.Substring(0, 10);
        }
        else
        {
            feature[nameof(oMapsuiPointClass.Title)] = oMapsuiPointClass.Title;
        }

        feature[nameof(oMapsuiPointClass.IsOk)] = oMapsuiPointClass.IsOk ? "OK" : "NOT OK";
        feature[nameof(oMapsuiPointClass.CreatedDateTime)] = oMapsuiPointClass.CreatedDateTime.ToString();

        return feature;
    }

    //todo: create a new pin on the map with specific style
    public static IFeature CreateFeatureOnTheMapWithSpecificStyle(MapsuiPointClass oMapsuiPointClass)
    {
        return CreateFeatureFromMapsuiPointClass(oMapsuiPointClass);
    }
}