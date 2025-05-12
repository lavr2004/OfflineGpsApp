using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Editing;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Tiling.Layers;
using nsMapsuiService = OfflineGpsApp.CodeBase.Services.MapsuiService;
using nsApp = OfflineGpsApp.CodeBase.App;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService;

/// <summary>
/// Creating and refreshing map with layers
/// Adding layers to map
/// Adding tracks to layers
/// </summary>
public class MapsuiService
{
    bool IsMapInitialized { get; set; } = false;

    public Mapsui.Map OMapsuiMap { get; set; } = new Mapsui.Map();
    private nsMapsuiService.Builders.LayersBuilder oLayersBuilder;

    bool isMapEmpty = false;

    public MapsuiService()
    {
        oLayersBuilder = new nsMapsuiService.Builders.LayersBuilder();
    }


    public async Task<Mapsui.Map> CreateMapAsync(List<nsMapsuiService.Models.MapsuiServicePointModel>? oModelList = null)
    {
        if (oModelList == null || oModelList.Count == 0) isMapEmpty = true;
        if (!IsMapInitialized)
        {
            TileLayer osmTileLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();

            OMapsuiMap.Layers.Add(osmTileLayer);

            if (oModelList != null)
            {
                MemoryLayer oMemoryLayer = oLayersBuilder.CreateMapsuiPointsLayer(oModelList);
                OMapsuiMap.Layers.Add(oMemoryLayer);

                //CenterMapOnPoints(oMemoryLayer);
                //todo: to define that like async call or dont?
                CenterMapOnPoints(oLayersBuilder.OTaskModelsMemoryLayer);
            }

            IsMapInitialized = true;
        }
        return OMapsuiMap;
    }

    public async Task<Mapsui.Map> RefreshMapAsync(List<nsMapsuiService.Models.MapsuiServicePointModel>? oModelList = null)
    {
        if (oModelList == null || oModelList.Count == 0)
        {
            isMapEmpty = true;
            return OMapsuiMap;
        };

        if (IsMapInitialized)
        {
            oLayersBuilder.UpdateLayerWithNewMapsuiPointModels(OMapsuiMap, nsMapsuiService.Settings.MapsuiServiceSettings.TaskModelsLayerTitle, oModelList);

            IsMapInitialized = true;
        }

        return OMapsuiMap;
    }

    public async Task<Mapsui.Map> CreateMapAsync(nsMapsuiService.Models.MapsuiServicePointModel oMapsuiPointClass)
    {
        if (!IsMapInitialized)
        {
            TileLayer osmTileLayer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer();
            OMapsuiMap.Layers.Add(osmTileLayer);

            MemoryLayer oMemoryLayer = oLayersBuilder.CreateMapsuiPointsLayer(new List<nsMapsuiService.Models.MapsuiServicePointModel> { oMapsuiPointClass });
            OMapsuiMap.Layers.Add(oMemoryLayer);

            //todo: to define that like async call or dont?
            CenterMapOnPoints(oLayersBuilder.OTaskModelsMemoryLayer);
        }

        IsMapInitialized = true;

        return OMapsuiMap;
    }

    /// <summary>
    /// Centering map on "points box" (MRect) inside the layer 
    /// </summary>
    /// <param name="oMemoryLayer"></param>
    public async Task CenterMapOnPoints(MemoryLayer oMemoryLayer)
    {
        // If dont exists geometry in layer, try to use GPS location
        if (isMapEmpty)
        {
            // Getting GPS location
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location != null)
            {
                //var point = new Mapsui.MPoint(location.Longitude, location.Latitude);
                var mercatorCoordinateXY = Mapsui.Projections.SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
                OMapsuiMap.Navigator.CenterOnAndZoomTo(
                    Mapsui.Extensions.TupleExtensions.ToMPoint(mercatorCoordinateXY),
                    OMapsuiMap.Navigator.Resolutions[10]
                );
            }
            else
            {
                // If no GPS location, center on 0,0
                OMapsuiMap.Navigator.CenterOnAndZoomTo(new Mapsui.MPoint(0, 0), OMapsuiMap.Navigator.Resolutions[10]);
            }

            return;
        }

        //If the layer has objects, center on them
        OMapsuiMap.Home = _ =>
        {


            // If the layer has just one object, center on it
            if (oMemoryLayer.Extent.Width == 0 && oMemoryLayer.Extent.Height == 0)
            {
                Mapsui.MPoint oMPoint = new Mapsui.MPoint(oMemoryLayer.Extent.MinX, oMemoryLayer.Extent.MinY);
                OMapsuiMap.Navigator.CenterOnAndZoomTo(oMPoint, OMapsuiMap.Navigator.Resolutions[10]);
                return;
            }

            // if the layer has more than one object, center on the layer "box"
            if (oMemoryLayer.Extent != null)
            {
                var extentMRect = oMemoryLayer.Extent!.Grow(oMemoryLayer.Extent.Width * 0.2);
                OMapsuiMap.Navigator.ZoomToBox(extentMRect);
                return;
            }

        };
    }

    public void AddNewTaskModelOnMap(nsMapsuiService.Models.MapsuiServicePointModel oModel, string oLayerName)
    {
        oLayersBuilder.AddPointToExistingLayer(OMapsuiMap, oModel, oLayerName);
    }
}