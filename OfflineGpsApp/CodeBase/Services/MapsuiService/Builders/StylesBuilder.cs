
using Mapsui.Styles;

namespace OfflineGpsApp.CodeBase.Services.MapsuiService.Builders;

public static class StylesBuilder
{
    /// <summary>
    /// Create a PinStyle for the map
    /// </summary>
    /// <returns></returns>
    public static Mapsui.Styles.SymbolStyle CreatePinStyle()
    {
        //creation blue pin for the map
        return Mapsui.Styles.SymbolStyles.CreatePinStyle(symbolScale: 0.7);

        //creation round pin for the map
        //Mapsui.Styles.SymbolStyle symbolStyle = new Mapsui.Styles.SymbolStyle();
        //symbolStyle.SymbolScale = 0.7f;
        //return symbolStyle;
    }


    /// <summary>
    /// A CalloutStyle shows a callout or InfoWindow in Google Maps
    /// https://mapsui.com/api/Mapsui.Styles.CalloutStyle.html
    /// </summary>
    /// <param name="content"></param>
    /// <returns>CalloutStyle</returns>
    public static Mapsui.Styles.CalloutStyle CreatePinCalloutStyle(string content)
    {
        //Offset of images from the center of the image. If IsRelative, than the offset is between -0.5 and +0.5.
        Mapsui.Styles.Offset offset = new Mapsui.Styles.Offset(0, Mapsui.Styles.SymbolStyle.DefaultHeight * 1f, isRelative: false);

        //Create callout up on featuer
        return new Mapsui.Styles.CalloutStyle
        {
            Title = content,//Content of Callout title label
            TitleFont = { FontFamily = null, Size = 12, Italic = false, Bold = true },
            TitleFontColor = Mapsui.Styles.Color.Gray,
            MaxWidth = 120,
            RectRadius = 10,
            ShadowWidth = 4,
            Enabled = false,
            SymbolOffset = offset
        };
    }
}