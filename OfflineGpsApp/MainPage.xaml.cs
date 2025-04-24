using Mapsui;
using Mapsui.UI.Maui;
using Microsoft.Maui.Controls;

namespace OfflineGpsApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        SetupMap();
    }

    private void SetupMap()
    {
        var map = new Mapsui.Map
        {
            CRS = "EPSG:3857" // Web Mercator для OSM
        };

        MapView.Map = map;
    }
}