using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using nsDatabaseService = OfflineGpsApp.CodeBase.Services.DatabaseService;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

public class MapsuiServicePoiModel
{
    nsDatabaseService.Models.DatabaseServiceMonumentModel oDatabaseServiceMonumentModel { get; set; }
    public MapsuiServicePoiModel(nsDatabaseService.Models.DatabaseServiceMonumentModel poi)
    {
        oDatabaseServiceMonumentModel = poi;
    }

    /// <summary>
    /// Method to convert the point into a Mapsui Feature from Mapsui.Layers.PointFeature
    /// </summary>
    /// <returns></returns>
    public Mapsui.Layers.PointFeature ToPointFeature()
    {
        //.ToMPoint() - is Mapsui.Extensions method
        Mapsui.Layers.PointFeature feature = new Mapsui.Layers.PointFeature(SphericalMercator.FromLonLat(oDatabaseServiceMonumentModel.Longitude, oDatabaseServiceMonumentModel.Latitude).ToMPoint());
        feature["Name"] = oDatabaseServiceMonumentModel.Name; //ADDED: add monument name for display
        feature.Styles = new System.Collections.Generic.List<Mapsui.Styles.IStyle>
            {
                new Mapsui.Styles.SymbolStyle
                {
                    SymbolScale = 0.5,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Black, 2)
                }
            };
        return feature;
    }
}
