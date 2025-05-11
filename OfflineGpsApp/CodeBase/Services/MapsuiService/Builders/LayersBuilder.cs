
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

using nsMapsuiService = OfflineGpsApp.CodeBase.Services.MapsuiService;
namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders;

/// <summary>
/// Delegate for adding properties to a feature on Callout
/// </summary>
/// <param name="feature"></param>
/// <param name="oMapsuiPointClass"></param>
/// <returns></returns>
public delegate Mapsui.IFeature AddPropertiesToFeatureDelegate(Mapsui.IFeature feature, nsMapsuiService.Models.MapsuiPointClass oMapsuiPointClass);

/// <summary>
/// Creating layers for map
/// Converting tracks into layers
/// </summary>
public class LayersBuilder
{
    public Mapsui.Layers.MemoryLayer OTaskModelsMemoryLayer { get; set; } = new Mapsui.Layers.MemoryLayer();


    /// <summary>
    /// Convert collection of points into into MemoryLayer with markers on the map
    /// </summary>
    /// <param name="oMapsuiPointClassList"></param>
    /// <returns></returns>
    public Mapsui.Layers.MemoryLayer CreateMapsuiPointsLayer(IEnumerable<nsMapsuiService.Models.MapsuiPointClass> oMapsuiPointClassList, AddPropertiesToFeatureDelegate? addPropsToFeatureMethod = null, bool isNewPinJustCreated = false)
    {
        string calloutContent;
        //conversion markers list into features on the map
        var oPointFeatures = oMapsuiPointClassList.Select(oMapsuiPointClass =>
        {
            Mapsui.IFeature oIFeature;

            if (!isNewPinJustCreated)
            {
                oIFeature = FeaturesBuilder.CreateFeatureFromMapsuiPointClass(oMapsuiPointClass);
            }
            else
            {
                //todo: create a new pin on the map with specific style
                oIFeature = FeaturesBuilder.CreateFeatureFromMapsuiPointClass(oMapsuiPointClass);
            }


            if (addPropsToFeatureMethod != null)
            {
                oIFeature = addPropsToFeatureMethod(oIFeature, oMapsuiPointClass);
            }

            //add callout style to the feature
            calloutContent = Mapsui.Extensions.FeatureExtensions.ToStringOfKeyValuePairs(oIFeature);
            oIFeature.Styles.Add(StylesBuilder.CreatePinCalloutStyle(calloutContent));
            return oIFeature;
        });


/* Unmerged change from project 'OfflineGpsApp (net8.0-windows10.0.19041.0)'
Before:
        OTaskModelsMemoryLayer = CreateMemoryLayer(oPointFeatures, OfflineGpsApp.CodeBase.Settings.GlobalSettings.TaskModelsLayerTitle);
After:
        OTaskModelsMemoryLayer = CreateMemoryLayer(oPointFeatures, GlobalSettings.TaskModelsLayerTitle);
*/
        OTaskModelsMemoryLayer = CreateMemoryLayer(oPointFeatures, nsMapsuiService.Settings.MapsuiServiceSettings.TaskModelsLayerTitle);

        return OTaskModelsMemoryLayer;
    }

    /// <summary>
    /// creation markers MemoryLayer for map from features
    /// </summary>
    /// <param name="oPointFeatures"></param>
    /// <returns></returns>
    public Mapsui.Layers.MemoryLayer CreateMemoryLayer(IEnumerable<Mapsui.IFeature?> oPointFeatures, string layerTitle)
    {
        if (oPointFeatures == null || oPointFeatures.Count() == 0) throw new ArgumentException("ER - oPointFeatures is null or empty");
        return new Mapsui.Layers.MemoryLayer()
        {
            Name = layerTitle,//"MarkersLayer",
            IsMapInfoLayer = true,
            Features = new MemoryProvider(oPointFeatures).Features,
            Style = StylesBuilder.CreatePinStyle(),//create style for every pin on layer
        };
    }

    public void UpdateLayerWithNewMapsuiPointModels(Mapsui.Map oMapsuiMap, string oNameLayerToUpdate, List<nsMapsuiService.Models.MapsuiPointClass> oMapsuiPointClassList)
    {
        if (oMapsuiPointClassList.Count == 0) return;

        MemoryLayer newMemoryLayer = CreateMapsuiPointsLayer(oMapsuiPointClassList);
        newMemoryLayer.Name = oNameLayerToUpdate; // Усталёўваем унікальнае імя для слоя

        ReplaceLayer(oMapsuiMap, oNameLayerToUpdate, newMemoryLayer);
    }

    /// <summary>
    /// Method for replacing layer by another layer by name
    /// </summary>
    /// <param name="oMapsuiMap"></param>
    /// <param name="oNameLayerToReplace"></param>
    /// <param name="newLayer"></param>
    private void ReplaceLayer(Mapsui.Map oMapsuiMap, string oNameLayerToReplace, ILayer newLayer)
    {
        var existingLayer = oMapsuiMap.Layers.FirstOrDefault(layer => layer.Name == oNameLayerToReplace);
        if (existingLayer != null)
        {
            oMapsuiMap.Layers.Remove(existingLayer);
        }
        newLayer.Name = oNameLayerToReplace; // Каб пазней зноў ідэнтыфікаваць
        oMapsuiMap.Layers.Add(newLayer);
    }

    public void AddPointToExistingLayer(Mapsui.Map map, nsMapsuiService.Models.MapsuiPointClass newMapsuiPoint, string layerName)
    {
        // Search via existing layers by name
        var existingLayer = map.Layers.FirstOrDefault(layer => layer.Name == layerName) as MemoryLayer;
        if (existingLayer == null)
        {
            throw new Exception($"ER: Layer '{layerName}' does not exists");
        }

        // Copying features from existing layer to a new list
        var existingFeatures = existingLayer.Features.ToList();

        // Creating a new feature from the new MapsuiPointClass
        var newFeature = FeaturesBuilder.CreateFeatureFromMapsuiPointClass(newMapsuiPoint);

        // Adding new feature to list
        existingFeatures.Add(newFeature);

        // Replace existing features with the new list
        existingLayer.Features = existingFeatures;

        // Refresh the map to show the new feature
        map.RefreshData();
    }

    public Mapsui.Layers.ILayer CreateLineStringLayerFromLatLonList(List<List<string>> latLonListList)
    {
        //var lineString = (LineString)new WKTReader().Read(WKTGr5);
        //lineString = new LineString(lineString.Coordinates.Select(v => SphericalMercator.FromLonLat(v.Y, v.X).ToCoordinate()).ToArray());

        NetTopologySuite.Geometries.LineString linestring = new NetTopologySuite.Geometries.LineString(latLonListList.Select(v => SphericalMercator.FromLonLat(double.Parse(v[1]), double.Parse(v[0])).ToCoordinate()).ToArray());

        return new MemoryLayer
        {
            Features = new[] { new GeometryFeature { Geometry = linestring } },
            Name = "LineStringLayer",
            Style = CreateLineStringStyle()

        };
    }

    //public static ILayer CreateLineStringLayer(string wellKnownText_wkt_format, Mapsui.Styles.IStyle? style = null)
    //{
    //    var lineString = (LineString)new WKTReader().Read(wellKnownText_wkt_format);
    //    lineString = new LineString(lineString.Coordinates.Select(v => SphericalMercator.FromLonLat(v.Y, v.X).ToCoordinate()).ToArray());

    //    return new MemoryLayer
    //    {
    //        Features = new[] { new GeometryFeature { Geometry = lineString } },
    //        Name = "LineStringLayer",
    //        Style = style

    //    };
    //}

    public static IStyle CreateLineStringStyle()
    {
        return new VectorStyle
        {
            Fill = null,
            Outline = null,
#pragma warning disable CS8670 // Object or collection initializer implicitly dereferences possibly null member.
            Line = { Color = Mapsui.Styles.Color.FromString("YellowGreen"), Width = 4 }
        };
    }
}