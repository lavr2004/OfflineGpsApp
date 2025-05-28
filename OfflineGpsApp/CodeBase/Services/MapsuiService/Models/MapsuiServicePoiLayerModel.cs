using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nsMapsuiService = OfflineGpsApp.CodeBase.Services.MapsuiService;
using nsDatabaseService = OfflineGpsApp.CodeBase.Services.DatabaseService;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Models;

/// <summary>
/// Class that represents a layer of Points of Interest (POI) on the map, such as monuments.
/// </summary>
public class MapsuiServicePoiLayerModel
{
    List<nsMapsuiService.Models.MapsuiServicePoiModel> ListMapsuiServicePoiModel { get; set; }
    string LayerName { get; set; }
    public MapsuiServicePoiLayerModel(List<nsMapsuiService.Models.MapsuiServicePoiModel> pois, string layername = "Monuments")
    {
        ListMapsuiServicePoiModel = pois;
        LayerName = layername;
    }
    public MapsuiServicePoiLayerModel(List<nsDatabaseService.Models.DatabaseServiceMonumentModel> monuments, string layername = "Monuments")
    {
        if (monuments == null || monuments.Count == 0) { throw new ArgumentException("Monuments list cannot be null or empty.", nameof(monuments)); }
        ListMapsuiServicePoiModel = monuments.Select(m => new nsMapsuiService.Models.MapsuiServicePoiModel(m)).ToList();
        LayerName = layername;
    }

    public Mapsui.Layers.MemoryLayer GetPoisLayer()
    {
        List<Mapsui.IFeature> features = new List<Mapsui.IFeature>();
        foreach (var poi in ListMapsuiServicePoiModel)
        {
            var pointFeature = poi.ToPointFeature();
            features.Add(pointFeature);
        }

        return new MemoryLayer()
        {
            Name = this.LayerName,//"MarkersLayer",
            IsMapInfoLayer = true,
            Features = new MemoryProvider(features).Features,
            Style = null,//setting up "null" to use specific style that every feature have inside self
            //Style = Mapsui.Styles.SymbolStyles.CreatePinStyle(symbolScale: 0.7),//create style for every pin on layer
        };
    }
}
